using UnityEngine;
using UnityEngine.UI;

public class PlayerSetting : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private ConfigAvatarData _avatarData;
    [SerializeField] private GameObject _colorPrefab;
    [SerializeField] private GameObject _avatarPrefab;
    [SerializeField] private TMPro.TMP_InputField _playerNameInputField;
    [SerializeField] private GridLayoutGroup _colorGridLayoutGroup;
    [SerializeField] private RawImage _colorPreview;
    [SerializeField] private RawImage _avatarPreview;
    [SerializeField] private Transform _avatarParent;

    private void Awake()
    {
        _colorPreview.color = CurrenPlayerData.Instance.Color;
        // clear all prefabs
        foreach (Transform child in _colorGridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        // clear avatar parent
        foreach (Transform child in _avatarParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var color in Constants.Colors)
        {
            var colorPrefab = Instantiate(_colorPrefab, _colorGridLayoutGroup.transform);
            var rawImage = colorPrefab.GetComponent<RawImage>();
            rawImage.color = color;
            colorPrefab.GetComponent<Button>().onClick.AddListener(() =>
            {
                _colorPreview.color = color;
                CurrenPlayerData.Instance.Color = color;
            });
        }
        
        for (int i = 0; i < _avatarData.avatars.Length; i++)
        {
            if (_avatarData.avatars[i].Available)
            {
                var index = i;
                var name = _avatarData.avatars[i].AvatarName;
                var avatarPrefab = Instantiate(_avatarPrefab, _avatarParent);
                var avatarImage = avatarPrefab.GetComponent<RawImage>();
                var image = _avatarData.avatars[i].AvatarSprite.texture;
                avatarImage.texture = image;


                avatarPrefab.GetComponent<Button>().onClick.AddListener(() =>
                {
                    CurrenPlayerData.Instance.Avatar = index;
                    _avatarPreview.texture = image;
                });
            }
        }

        // set default
        CurrenPlayerData.Instance.Color = Color.white;
        string _name = NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(7));
        CurrenPlayerData.Instance.Name = _name;
        _playerNameInputField.text = _name;
    }

    public void OnPlayerNameEntered()
    {
        CurrenPlayerData.Instance.Name = _playerNameInputField.text;
    }

    public void RandomName()
    {
        string _name = NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(7));
        CurrenPlayerData.Instance.Name = _name;
        _playerNameInputField.text = _name;
    }
}
