using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarData", menuName = "Avatar/Avatar Data")]
public class ConfigAvatarData : ScriptableObject
{
    public AvatarData[] avatars;

    public AvatarData GetAvatar(int avatar)
    {
        return avatars[avatar];
    }

    public int Count()
    {
        return avatars.Length;
    }
}