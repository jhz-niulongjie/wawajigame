using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public sealed class UIAtlasManager:Singleton<UIAtlasManager>
{
    private const string path = "UIAtlas/";
   
    private Dictionary<string, Sprite[]> atlasDic = new Dictionary<string, Sprite[]>();

    public Sprite LoadSprite(string atlasName, string spriteName)
    {
        Sprite sp = FindSprite(atlasName, spriteName);
        if (sp == null)
        {
            string newPath = path + atlasName;
            Sprite[] sps = Resources.LoadAll<Sprite>(newPath);
            sp = GetSpriteForAtlas(sps, spriteName);
            atlasDic.Add(atlasName, sps);
        }
        return sp;
    }



    private Sprite FindSprite(string atlasName, string spriteName)
    {
        if (atlasDic.ContainsKey(atlasName))
        {
            Sprite[] sp = atlasDic[atlasName];
            return GetSpriteForAtlas(sp, spriteName);
        }
        return null;
    }

    private Sprite GetSpriteForAtlas(Sprite[] sps, string spriteName)
    {
        for (int i = 0; i < sps.Length; i++)
        {
            if (sps[i].GetType() == typeof(Sprite))
            {
                if (sps[i].name == spriteName)
                    return sps[i];
            }
        }
        Debug.LogWarning("图片名:" + spriteName + ";在图集中找不到");
        return null;
    }

    public void Clear()
    {
        foreach (var item in atlasDic)
        {
            foreach (var sp in item.Value)
            {
                Resources.UnloadAsset(sp);
            }
        }
        atlasDic.Clear();
        Dispose();
    }
}
