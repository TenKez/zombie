using UnityEngine;

public enum VoiceChannel { Proximity, Walkie }

public class VoiceChatInterface : MonoBehaviour
{
    // This is an abstraction placeholder. Hook it up to Vivox or Dissonance.
    // Provide functions to set the current channel and emit "noise" to NoiseSystem while speaking.

    public VoiceChannel channel;
    public int walkieFrequency = 101; // players must match to hear each other

    private bool _speaking;

    public void SetSpeaking(bool speaking)
    {
        _speaking = speaking;
        if (speaking)
        {
            // TODO: Query input mic volume; for now, emit a fixed noise magnitude.
            NoiseSystem.Emit(transform.position, channel == VoiceChannel.Proximity ? 0.6f : 0.3f);
        }
    }

    public void SetWalkieFrequency(int freq) { walkieFrequency = freq; }
    public void SetChannel(VoiceChannel ch) { channel = ch; }
}
