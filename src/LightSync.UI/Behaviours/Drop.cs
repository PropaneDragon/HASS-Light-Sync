using Avalonia;
using Avalonia.Controls;
using System.Windows.Input;

namespace LightSync.UI.Behaviours
{
    public abstract class Drop : AvaloniaObject
    {
        public static readonly AttachedProperty<ICommand?> CommandProperty = AvaloniaProperty.RegisterAttached<Drop, Control, ICommand?>("Command");
        public static readonly AttachedProperty<ICommand?> OverCommandProperty = AvaloniaProperty.RegisterAttached<Drop, Control, ICommand?>("OverCommand");
        public static readonly AttachedProperty<bool> IsOverProperty = AvaloniaProperty.RegisterAttached<Drop, Control, bool>("IsOver");

        static Drop()
        {

        }

        public static ICommand? GetCommand(Control control) => control.GetValue(CommandProperty);
        public static void SetCommand(Control control, ICommand? command) => control.SetValue(CommandProperty, command);

        public static ICommand? GetOverCommand(Control control) => control.GetValue(OverCommandProperty);
        public static void SetOverCommand(Control control, ICommand? command) => control.SetValue(OverCommandProperty, command);

        public static bool GetIsOver(Control control) => control.GetValue(IsOverProperty);
        public static void SetIsOver(Control control, bool isOver) => control.SetValue(IsOverProperty, isOver);
    }
}
