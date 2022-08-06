using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Menus;

RenderWindow window = new RenderWindow(new VideoMode(1000, 500), "Test", Styles.Fullscreen);

StartMenu startMenu = new StartMenu(window);

IReadOnlyList<Drawable> drawables = startMenu.Items;

RectangleShape bg = new RectangleShape();
bg.FillColor = new Color(235, 255, 255);
bg.Size = new Vector2f(window.Size.X, window.Size.Y);

while (window.IsOpen)
{
    window.Clear();
    window.Draw(bg);
    foreach (Drawable drawable in drawables)
        window.Draw(drawable);
    window.Display();
    window.WaitAndDispatchEvents();
}