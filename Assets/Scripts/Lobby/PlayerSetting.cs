using UnityEngine;
using UnityEngine.UI;

public class PlayerSetting : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject _colorPrefab;
    [SerializeField] private TMPro.TMP_InputField _playerNameInputField;
    [SerializeField] private GridLayoutGroup _colorGridLayoutGroup;
    [SerializeField] private RawImage _colorPreview;

    private void Awake()
    {
        _colorPreview.color = CurrenPlayerData.Instance.Color;
        // clear all prefabs
        foreach (Transform child in _colorGridLayoutGroup.transform)
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
