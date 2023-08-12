using UnityEngine;
using UnityEngine.UI;

public class PlayerSetting : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject _colorPrefab;
    [SerializeField] private GameObject _avatarPrefab;
    [SerializeField] private TMPro.TMP_InputField _playerNameInputField;
    [SerializeField] private GridLayoutGroup _colorGridLayoutGroup;
    [SerializeField] private RawImage _colorPreview;
    [SerializeField] private RawImage _avatarPreview;
    [SerializeField] private Transform _avatarParent;

    private void OnEnable()
    {
        foreach (Transform child in _colorGridLayoutGroup.transform) Destroy(child.gameObject);
        foreach (Transform child in _avatarParent) Destroy(child.gameObject);

        _colorPreview.color = CurrenPlayerData.Instance.Color;
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

        _avatarPreview.texture = GlobalVariable.Instance.GetAvatar(CurrenPlayerData.Instance.Avatar).AvatarSprite.texture;
        for (int i = 0; i < GlobalVariable.Instance.AvatarCount(); i++)
        {
            if (GlobalVariable.Instance.GetAvatar(i).Available)
            {
                var index = i;
                var name = GlobalVariable.Instance.GetAvatar(i).AvatarName;
                var avatarPrefab = Instantiate(_avatarPrefab, _avatarParent);
                var avatarImage = GlobalVariable.Instance.GetAvatar(i).AvatarSprite.texture;
                avatarPrefab.GetComponent<RawImage>().texture = avatarImage;

                avatarPrefab.GetComponent<Button>().onClick.AddListener(() =>
                {
                    CurrenPlayerData.Instance.Avatar = index;
                    _avatarPreview.texture = avatarImage;
                });
            }
        }

        // set default
        CurrenPlayerData.Instance.Color = CurrenPlayerData.Instance.Color == Color.white ? Color.white : CurrenPlayerData.Instance.Color;
        string _name = NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(5));
        CurrenPlayerData.Instance.Name = CurrenPlayerData.Instance.Name == "Unknown" ? _name : CurrenPlayerData.Instance.Name;
        _playerNameInputField.text = CurrenPlayerData.Instance.Name;
    }

    private void OnDisable()
    {
        foreach (Transform child in _colorGridLayoutGroup.transform) Destroy(child.gameObject);
        foreach (Transform child in _avatarParent) Destroy(child.gameObject);
    }

    public void OnPlayerNameEntered()
    {
        CurrenPlayerData.Instance.Name = _playerNameInputField.text;
    }

    public void RandomName()
    {
        string _name = NVJOBNameGen.Uppercase(NVJOBNameGen.GiveAName(5));
        CurrenPlayerData.Instance.Name = _name;
        _playerNameInputField.text = _name;
    }
}
