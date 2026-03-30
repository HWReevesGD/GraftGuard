namespace GraftGuard.UI;
internal interface IMouseDetectable
{
    /// <summary>
    /// Checks if the Mouse is over the object given the current <paramref name="inputManager"/> state
    /// </summary>
    /// <param name="inputManager"><see cref="InputManager"/> to use for Mouse</param>
    /// <returns><see cref="true"/> if the Mouse is over this object, <see cref="false"/> otherwise</returns>
    public bool IsMouseOver(InputManager inputManager);
}
