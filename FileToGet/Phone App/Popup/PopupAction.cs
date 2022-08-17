using System;

namespace Marbotic.Framework.UI {

  public readonly struct PopupAction {

    public readonly string title;
    public readonly Action<Popup> action;

    public PopupAction(string title, Action<Popup> action) {
      this.title = title;
      this.action = action;
    }

    public static implicit operator PopupAction(string title) => new(title, null);

    public static implicit operator PopupAction(Action action) => new(string.Empty, _ => action?.Invoke());
    public static implicit operator PopupAction(Action<Popup> action) => new(string.Empty, action);

    public static implicit operator PopupAction(ValueTuple<string, Action> action) => new(action.Item1, _ => action.Item2?.Invoke());
    public static implicit operator PopupAction(ValueTuple<string, Action<Popup>> action) => new(action.Item1, action.Item2);
  }
}
