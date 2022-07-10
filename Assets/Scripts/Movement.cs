using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Movement : MonoBehaviourPunCallbacks
{
    // 컴포넌트 캐시 처리를 위한 변수
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private Fire fire;
    private new Camera camera;

    // 가상의 Plane에 레이캐스팅하기 위한 변수
    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    // PhotonView 컴포넌트 캐시처리를 위한 변수
    private PhotonView pv;

    // 시네머신 가상 카메라를 저장할 변수
    // private CinemachineVirtualCamera virtualCamera;

    // 이동 속도
    public float moveSpeed = 10.0f;

    // 수신된 위치와 회전값을 저장할 변수
    private Vector3 receivePos;
    private Quaternion receiveRot;
    // 수신된 좌표로 이동 및 회전 속도의 민감도
    public float damping = 10.0f;

    private float m_fMouseX = 0f;

    // 슈팅 딜레이 추가
    public float fireRate = 0.3F;
    private float nextFire = 0.0F;

    public VirtualJoystick joystick;
    private Image bgImg;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        fire = GetComponent<Fire>();

        // Find joystick
        GameObject tmp = GameObject.Find("BackgroundImage");
        joystick = tmp.GetComponent<VirtualJoystick>();

        bgImg = tmp.GetComponent<Image>();



        //camera = Camera.main;
        
        GameObject temp = GameObject.Find("PlayerCamera");
        camera = temp.GetComponent<Camera>();

        pv = GetComponent<PhotonView>();


        // 추가 항목
        m_fMouseX = 0f;

        
        // 가상의 바닥을 주인공의 위치를 기준으로 생성
        plane = new Plane(transform.up, transform.position);
    }

    void Update()
    {
        // 자신이 생성한 네트워크 객체만 컨트롤
        if (pv.IsMine)
        {
            Move();
            Turn();
            Shoot();
        }
    }

    // 조이스틱 입력값 연결
    float h => joystick.Horizontal();
    float v => joystick.Vertical();

    // 이동 처리하는 함수
    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        //Debug.Log("카메라 출력 ######"+ camera.transform.forward + ", " + camera.transform.right);

        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);


        moveDir.Set(moveDir.x, 0.0f, moveDir.z);


        //Debug.Log("Vector to Move : " + moveDir * moveSpeed);

        // 주인공 캐릭터 이동처리
        controller.SimpleMove(moveDir * moveSpeed);

        // 주인공 캐릭터의 애니메이션 처리
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);

    }

    // 회전 처리하는 함수
    void Turn()
    {

        // 터치 후 움직이면 화면 이동

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {


                if (touch.phase == TouchPhase.Moved)
                {
                    //영역 밖일 때만
                    //Debug.Log(bgImg.rectTransform.anchoredPosition+", "+ joystickImg.rectTransform.anchoredPosition);
                    //Vector2 pos;
                    //if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, touch.position-touch.deltaPosition, camera, out pos))
                    //{
                        m_fMouseX += touch.deltaPosition.x;
                        transform.localRotation = Quaternion.Euler(0, m_fMouseX, 0);

                    //}
                    //Debug.Log(touch.rawPosition);
                }
            }
        }

        //m_fMouseX += Input.GetAxis("Mouse X") * 5.0f;
        //transform.localRotation = Quaternion.Euler(0, m_fMouseX, 0);
    }
    void Shoot()
    {
        if (pv.IsMine)
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase != TouchPhase.Moved && Time.time > nextFire)
                    {
                        //Vector2 pos;
                        //if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, touch.position - touch.deltaPosition, camera, out pos))
                        //{
                            // Began
                            nextFire = Time.time + fireRate;

                            fire.FireBullet(pv.Owner.ActorNumber);
                            //RPC로 원격지에 있는 함수를 호출
                            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber);
                        //}
                    }
                }
            }
        }
    }
}
