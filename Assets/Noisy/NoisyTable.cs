using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class NoisyTable : UdonSharpBehaviour
{
    public Transform snapPoint;
    public AudioSource[] sources = new AudioSource[3];

    [UdonSynced]
    private int playingMask;

    private void Start()
    {
        ApplyAudioVolumes();
    }

    private void OnTriggerEnter(Collider other)
    {
        NoisyBlock b = other.GetComponent<NoisyBlock>();
        if (b != null) {
            b.noisyTable = this;
            b.attached = true;
            b.RefreshColor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NoisyBlock b = other.GetComponent<NoisyBlock>();
        if (b != null && b.noisyTable == this) {
            b.attached = false;
            b.RefreshColor();
            Debug.Log("OnTriggerExit (NoisyBlock)");
        }
    }

    public override void OnDeserialization()
    {
        ApplyAudioVolumes();
    }

    public void SetBlockPlaying(NoisyBlock b, bool shouldPlay)
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        int blockType = b.GetBlockTypeIndex();
        int blockMask = 1 << blockType;

        if (shouldPlay)
        {
            playingMask |= blockMask;
        }
        else
        {
            playingMask &= ~blockMask;
        }

        ApplyAudioVolumes();
        RequestSerialization();
    }

    public void SnapBlock(NoisyBlock b)
    {
        Vector3 position = b.transform.position;
        Vector3 blockAngles = b.transform.eulerAngles;

        if (snapPoint != null)
        {
            Vector3 snapAngles = snapPoint.eulerAngles;

            position.y = snapPoint.position.y;
            b.transform.position = position;
            b.transform.rotation = Quaternion.Euler(snapAngles.x, blockAngles.y, snapAngles.z);
        }
        else
        {
            Vector3 tableAngles = transform.eulerAngles;
            b.transform.rotation = Quaternion.Euler(tableAngles.x, blockAngles.y, tableAngles.z);
        }

        if (b.rb != null)
        {
            b.rb.velocity = Vector3.zero;
            b.rb.angularVelocity = Vector3.zero;
        }
    }

    private void ApplyAudioVolumes()
    {
        if (sources == null)
        {
            return;
        }

        for (int i = 0; i < sources.Length; i++)
        {
            AudioSource audioSource = sources[i];
            if (audioSource == null)
            {
                continue;
            }

            audioSource.volume = (playingMask & (1 << i)) != 0 ? 1f : 0f;
        }
    }

}
