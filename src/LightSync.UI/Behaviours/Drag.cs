using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;

namespace LightSync.UI.Behaviours
{
    internal abstract class Drag : AvaloniaObject
    {
        private const double _dragThreshold = 6;

        private static DragSession? _dragSession;

        private static readonly EventHandler<PointerPressedEventArgs> PressedHandler = OnPressed;
        private static readonly EventHandler<PointerEventArgs> MovedHandler = OnMoved;
        private static readonly EventHandler<PointerReleasedEventArgs> ReleasedHandler = OnReleased;
        private static readonly EventHandler<KeyEventArgs> KeyHandler = OnKeyDown;
        private static readonly EventHandler<PointerCaptureLostEventArgs> LostHandler = OnCaptureLost;

        public static readonly AttachedProperty<bool> CanDragProperty = AvaloniaProperty.RegisterAttached<Drag, Control, bool>("CanDrag");
        public static readonly AttachedProperty<object?> DataProperty = AvaloniaProperty.RegisterAttached<Drag, Control, object?>("Data");
        public static readonly AttachedProperty<IDataTemplate?> GhostTemplateProperty = AvaloniaProperty.RegisterAttached<Drag, Control, IDataTemplate?>("GhostTemplate");

        static Drag()
        {
            CanDragProperty.Changed.AddClassHandler<Control>((control, e) =>
            {
                control.RemoveHandler(InputElement.PointerPressedEvent, PressedHandler);

                if (e.NewValue == null)
                    return;

                control.AddHandler(InputElement.PointerPressedEvent, PressedHandler, Avalonia.Interactivity.RoutingStrategies.Tunnel, handledEventsToo: true);
            });
        }

        public static bool GetCanDrag(Control control) => control.GetValue(CanDragProperty);
        public static void SetCanDrag(Control control, bool canDrag) => control.SetValue(CanDragProperty, canDrag);

        public static object? GetData(Control control) => control.GetValue(DataProperty);
        public static void SetData(Control control, object? data) => control.SetValue(DataProperty, data);

        public static IDataTemplate? GetGhostTemplate(Control control) => control.GetValue(GhostTemplateProperty);
        public static void SetGhostTemplate(Control control, IDataTemplate? ghostTemplate) => control.SetValue(GhostTemplateProperty, ghostTemplate);

        private static void UpdateTarget(DragSession dragSession, PointerEventArgs e)
        {
            var foundTarget = FindTarget(dragSession.TopLevel, e.GetPosition(dragSession.TopLevel));
            var dropInfo = foundTarget == null ? null : new DropInfo(GetData(dragSession.Source), foundTarget, e.GetPosition(foundTarget));
            var overCommand = foundTarget == null ? null : Drop.GetOverCommand(foundTarget);
            var acceptsDrop = foundTarget != null && dropInfo != null && overCommand != null && (overCommand != null || (Drop.GetCommand(foundTarget)?.CanExecute(dropInfo) ?? false));
            var updatedTarget = acceptsDrop ? foundTarget : null;

            if (!ReferenceEquals(updatedTarget, dragSession.Target))
            {
                if (dragSession.Target != null)
                    Drop.SetIsOver(dragSession.Target, false);

                if (updatedTarget != null)
                    Drop.SetIsOver(updatedTarget, true);

                dragSession.Target = updatedTarget;
            }

            if (updatedTarget != null && overCommand != null && overCommand.CanExecute(dropInfo))
                overCommand.Execute(dropInfo);
        }

        private static Control? FindTarget(TopLevel top, Point point)
        {
            if (top.InputHitTest(point) is not Visual hit)
                return null;

            foreach (var visual in hit.GetSelfAndVisualAncestors())
            {
                if (visual is Control control && (Drop.GetCommand(control) != null || Drop.GetOverCommand(control) != null))
                    return control;
            }

            return null;
        }

        private static void AddHandlers(Control control, TopLevel top)
        {
            top.AddHandler(InputElement.PointerMovedEvent, MovedHandler, Avalonia.Interactivity.RoutingStrategies.Bubble, handledEventsToo: true);
            top.AddHandler(InputElement.PointerReleasedEvent, ReleasedHandler, Avalonia.Interactivity.RoutingStrategies.Tunnel, handledEventsToo: true);
            top.AddHandler(InputElement.KeyDownEvent, KeyHandler, Avalonia.Interactivity.RoutingStrategies.Tunnel, handledEventsToo: true);

            control.AddHandler(InputElement.PointerCaptureLostEvent, LostHandler);
        }

        private static void RemoveHandlers(Control control, TopLevel top)
        {
            top.RemoveHandler(InputElement.PointerMovedEvent, MovedHandler);
            top.RemoveHandler(InputElement.PointerReleasedEvent, ReleasedHandler);
            top.RemoveHandler(InputElement.KeyDownEvent, KeyHandler);

            control.RemoveHandler(InputElement.PointerCaptureLostEvent, LostHandler);
        }

        private static void EndSession(DragSession session)
        {
            if (!ReferenceEquals(session, _dragSession))
                return;

            if (_dragSession == null)
                return;

            _dragSession = null;

            RemoveHandlers(session.Source, session.TopLevel);

            if (session.Target != null)
                Drop.SetIsOver(session.Target, false);

            if (session.Ghost != null)
                session.Layer?.Children.Remove(session.Ghost);

            if (ReferenceEquals(session.Pointer.Captured, session.Source))
                session.Pointer.Capture(null);
        }

        private static void ShowGhost(DragSession dragSession)
        {
            dragSession.Layer = OverlayLayer.GetOverlayLayer(dragSession.TopLevel);
            if (dragSession.Layer == null)
                return;

            dragSession.Ghost = GetGhostTemplate(dragSession.Source) is { } template
                ? new ContentControl
                {
                    Content = GetData(dragSession.Source),
                    ContentTemplate = template
                }
                : new Border
                {
                    Width = dragSession.Source.Bounds.Width,
                    Height = dragSession.Source.Bounds.Height,
                    Background = new VisualBrush
                    {
                        Visual = dragSession.Source,
                        Stretch = Stretch.None,
                    }
                };

            dragSession.Ghost.IsHitTestVisible = false;
            dragSession.Ghost.Opacity = 0.8;
            dragSession.Ghost.Classes.Add("drag-ghost");
            dragSession.Layer.Children.Add(dragSession.Ghost);
        }

        private static void OnPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_dragSession != null || sender is not Control control)
                return;

            if (!e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                return;

            if (!GetCanDrag(control))
                return;

            if (TopLevel.GetTopLevel(control) is not { } top)
                return;

            _dragSession = new(control, top, e.GetPosition(top), e.GetPosition(control), e.Pointer);
            
            AddHandlers(control, top);
        }

        private static void OnMoved(object? sender, PointerEventArgs e)
        {
            if (_dragSession == null)
                return;

            if (!_dragSession.IsDragging)
            {
                var position = e.GetPosition(_dragSession.TopLevel);
                var distance = position - _dragSession.Start;

                distance.Deconstruct(out var x, out var y);

                if (x * x + y * y < _dragThreshold * _dragThreshold)
                    return;

                _dragSession.IsDragging = true;

                e.Pointer.Capture(_dragSession.Source);

                ShowGhost(_dragSession);
            }

            if (_dragSession.Ghost != null && _dragSession.Layer != null)
            {
                var position = e.GetPosition(_dragSession.Layer) - _dragSession.ControlOffset;

                Canvas.SetLeft(_dragSession.Ghost, position.X);
                Canvas.SetTop(_dragSession.Ghost, position.Y);
            }

            UpdateTarget(_dragSession, e);
        }

        private static void OnReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_dragSession == null)
                return;

            if (_dragSession.IsDragging)
            {
                e.Handled = true;

                var foundTarget = FindTarget(_dragSession.TopLevel, e.GetPosition(_dragSession.TopLevel));
                var dropCommand = foundTarget != null ? Drop.GetCommand(foundTarget) : null;
                var dropInfo = foundTarget == null ? null : new DropInfo(GetData(_dragSession.Source), foundTarget, e.GetPosition(foundTarget));

                if (dropCommand != null && dropCommand.CanExecute(dropInfo))
                    dropCommand.Execute(dropInfo);
            }

            EndSession(_dragSession);
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape || _dragSession == null)
                return;

            e.Handled = true;

            EndSession(_dragSession);
        }

        private static void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e)
        {
            if (_dragSession == null)
                return;

            EndSession(_dragSession);
        }

        private sealed class DragSession(Control source, TopLevel topLevel, Point start, Point controlOffset, IPointer pointer)
        {
            public bool IsDragging { get; set; }
            public Control Source { get; } = source;
            public Control? Target { get; set; }
            public Control? Ghost { get; set; }
            public TopLevel TopLevel { get; } = topLevel;
            public OverlayLayer? Layer { get; set; }
            public Point Start { get; } = start;
            public Point ControlOffset { get; } = controlOffset;
            public IPointer Pointer { get; } = pointer;
        }
    }

    public sealed record DropInfo(object? Data, Control Target, Point Position)
    {
    }
}
