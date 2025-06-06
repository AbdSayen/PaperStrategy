using System.Linq;
using UnityEngine;
using Photon.Pun;

public abstract class Source : Module
{
    public abstract string MaterialName { get; protected set; }

    private SpriteRenderer displayImage;
    private bool displayImageInitialized;

    protected override void Awake()
    {
        base.Awake();
        FindDisplayImage();
    }

    protected override void Start()
    {
        base.Start();
        if (photonView.IsMine)
        {
            OwnerId = null;
            photonView.RPC("SyncOwnerRPC", RpcTarget.OthersBuffered, (string)null);
        }
    }

    private void FindDisplayImage()
    {
        if (displayImage == null)
        {
            var childRenderers = GetComponentsInChildren<SpriteRenderer>(true)
                                .Where(sr => sr.transform != transform)
                                .ToList();

            if (childRenderers.Count > 0)
            {
                displayImage = childRenderers[0];
                displayImageInitialized = true;
                photonView.RPC("SyncDisplayImageColorRPC", RpcTarget.Others,
                             NormalColor.r, NormalColor.g, NormalColor.b, NormalColor.a);
            }
        }
        else
        {
            displayImageInitialized = true;
        }
    }

    public override void AutoSetColor()
    {
        base.AutoSetColor();

        if (!displayImageInitialized)
            FindDisplayImage();

        if (displayImage != null)
        {
            displayImage.color = NormalColor;
            if (PhotonNetwork.IsConnected)
                photonView.RPC("SyncDisplayImageColorRPC", RpcTarget.Others,
                             NormalColor.r, NormalColor.g, NormalColor.b, NormalColor.a);
        }
    }

    [PunRPC]
    private void SyncDisplayImageColorRPC(float r, float g, float b, float a)
    {
        if (displayImage != null)
        {
            displayImage.color = new Color(r, g, b, a);
        }
    }

    [PunRPC]
    private void SyncOwnerRPC(string ownerId)
    {
        OwnerId = ownerId;
        AutoSetColor();
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        if (stream.IsWriting)
        {
            stream.SendNext(displayImageInitialized);
            if (displayImageInitialized && displayImage != null)
            {
                stream.SendNext(displayImage.color.r);
                stream.SendNext(displayImage.color.g);
                stream.SendNext(displayImage.color.b);
                stream.SendNext(displayImage.color.a);
            }
        }
        else
        {
            bool hasDisplayImage = (bool)stream.ReceiveNext();
            if (hasDisplayImage)
            {
                float r = (float)stream.ReceiveNext();
                float g = (float)stream.ReceiveNext();
                float b = (float)stream.ReceiveNext();
                float a = (float)stream.ReceiveNext();

                if (displayImage != null)
                    displayImage.color = new Color(r, g, b, a);
            }
        }
    }
}