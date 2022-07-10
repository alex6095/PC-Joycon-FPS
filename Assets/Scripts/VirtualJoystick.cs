using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImg;
    private Image joystickImg;
    private Vector3 inputVector;


    private void Start()
    {
        bgImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        // 사각형 영역 확인
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            //Debug.Log("Hey what's up");
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            //float radius = (bgImg.rectTransform.sizeDelta.x - joystickImg.rectTransform.sizeDelta.x) / bgImg.rectTransform.sizeDelta.x;

            //Debug.Log(bgImg.rectTransform.sizeDelta.x +", "+ joystickImg.rectTransform.sizeDelta.x);

            // 알아서 원 안으로 화면에 맞추기
            inputVector = new Vector3(pos.x * 2 - 1, 0, pos.y * 2 - 1);
            //inputVector = (inputVector.magnitude > radius) ? inputVector.normalized*radius : inputVector;



            // 원밖으로 나간 것을 
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            //Debug.Log(inputVector);
            //Debug.Log(pos);

            // Move Joystick IMG
            joystickImg.rectTransform.anchoredPosition =
                new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 2), inputVector.z * (bgImg.rectTransform.sizeDelta.y / 2));
            //Debug.Log(bgImg.rectTransform.anchoredPosition+", "+ joystickImg.rectTransform.anchoredPosition);
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }

    public float Horizontal()  {
        //if (inputVector.x != 0)
        //    return inputVector.x;
        //else return Input.GetAxis("Horizontal");

        // 조이스틱 안눌려 있을 때는 0을 보내야 move가 안될것
        return inputVector.x;
    }

    public float Vertical()
    {
        //if (inputVector.z != 0)
        //    return inputVector.z;
        //else return Input.GetAxis("Vertical");

        // 조이스틱 안눌려 있을 때는 0을 보내야 move가 안될것

        return inputVector.z;
    }

}
