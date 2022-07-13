using UnityEngine;

namespace Match3
{
    public static class ItemDatabase 
    {
        public static Item[] Items {get; private set;}

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] //force to run this method on scene is loaded
        private static void Initialize() => Items = Resources.LoadAll<Item>(path:"Items/"); //load all Item Scriptable Objects from the Items folder
    }
}
