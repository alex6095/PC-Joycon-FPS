using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkCharacter : MonoBehaviour, IPunObservable
{
    private new Transform transform;

    // 수신된 좌표로 이동 및 회전 속도의 민감도
    public float damping = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    //void Update()
    //{
        


    //}
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 자신의 로컬 캐릭터인 경우 자신의 데이터를 다른 네트워크 유저에게 송신 
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            
            // 수신된 회전값으로 보간한 회전처리
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position,
                                              (Vector3)stream.ReceiveNext(),
                                              Time.deltaTime * damping), Quaternion.Slerp(transform.rotation,
                                                  (Quaternion)stream.ReceiveNext(),
                                                  Time.deltaTime * damping));
        }
    }
}
