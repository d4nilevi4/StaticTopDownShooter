namespace Shooter;

public static class GameObjectExtensions
{
    public static void RemoveCloneSuffix(this GameObject gameObject)
    {
        if (gameObject.name.EndsWith("(Clone)"))
        {
            gameObject.name = gameObject.name.Substring(0, gameObject.name.Length - 7);
        }
    }

    public static void RemoveCloneSuffix(this Component gameObject)
    {
        if (gameObject.gameObject.name.EndsWith("(Clone)"))
        {
            gameObject.gameObject.name =
                gameObject.gameObject.name.Substring(0, gameObject.gameObject.name.Length - 7);
        }
    }
}