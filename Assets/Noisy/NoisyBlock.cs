using UdonSharp;
using UnityEngine;

public class NoisyBlock : UdonSharpBehaviour
{
    public int blockType;
    public NoisyTable noisyTable = null;
    public bool attached = false;
    public Rigidbody rb;
    public MeshRenderer blockRenderer;
    public Color touchingColor = Color.green;

    private Color originalColor;
    private bool hasOriginalColor;
    private bool held;

    private void Start()
    {
        StoreOriginalColor();
        RefreshColor();
    }

    public MeshRenderer GetBlockRenderer()
    {
        if (blockRenderer == null)
        {
            blockRenderer = GetComponent<MeshRenderer>();
        }

        if (blockRenderer == null)
        {
            blockRenderer = GetComponentInChildren<MeshRenderer>();
        }

        return blockRenderer;
    }

    public void RefreshColor()
    {
        if (attached && held)
        {
            ApplyColor(touchingColor);
            return;
        }

        ApplyOriginalColor();
    }

    public override void OnPickup()
    {
        held = true;
        RefreshColor();
    }

    public override void OnDrop()
    {
        held = false;
        RefreshColor();

        Debug.Log("Dropped cube");
        if (noisyTable!=null && attached==true) {
            noisyTable.SnapBlock(this);
            noisyTable.SetBlockPlaying(this, true);
        }
        if (noisyTable!=null && attached==false) {
            noisyTable.SetBlockPlaying(this, false);
            noisyTable = null;
        }
    }

    private void StoreOriginalColor()
    {
        MeshRenderer renderer = GetBlockRenderer();
        if (renderer == null)
        {
            return;
        }

        originalColor = renderer.material.color;
        hasOriginalColor = true;
    }

    private void ApplyOriginalColor()
    {
        if (!hasOriginalColor)
        {
            StoreOriginalColor();
        }

        if (hasOriginalColor)
        {
            ApplyColor(originalColor);
        }
    }

    private void ApplyColor(Color color)
    {
        MeshRenderer renderer = GetBlockRenderer();
        if (renderer == null)
        {
            return;
        }

        renderer.material.color = color;
    }

    public int GetBlockTypeIndex()
    {
        if (blockType < 0)
        {
            return 0;
        }

        if (blockType > 2)
        {
            return 2;
        }

        return blockType;
    }
}
