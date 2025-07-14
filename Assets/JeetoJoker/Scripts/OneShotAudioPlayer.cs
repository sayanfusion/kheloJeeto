using UnityEngine;
using UnityEngine.UI;
using SoundControllerJeeto = JeetoJoker.SoundController;
public class OneShotAudioPlayer : MonoBehaviour
{
    [SerializeField] private SoundControllerJeeto.SoundType soundType;
    [SerializeField] private Button[] buttons;
    private int soundIndex;
    private void Start()
    {
        soundIndex = (int)soundType;
        int length = buttons.Length;
        for (int i = 0; i < length; i++)
        {
            buttons[i].onClick.AddListener(Play);
        }

    }

    private void Play()
    {
        SoundControllerJeeto.Instance.PlayOneShot(soundType, soundIndex);
    }
}
