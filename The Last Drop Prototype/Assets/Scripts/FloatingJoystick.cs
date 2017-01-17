using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FloatingJoystick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

	private Image bgImg;
	private Image joystickImg;

	public Vector2 inputDirection{ set; get; }

	void Start()
	{
		bgImg = GetComponent<Image>();
		joystickImg = transform.GetChild(0).GetComponent<Image>();
	}

	public void OnDrag(PointerEventData ped)
	{
		Vector2 pos = Vector2.zero;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle
			(bgImg.rectTransform, 
				ped.position,
				ped.pressEventCamera,
				out pos))
		{
			pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
			float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
			//pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);
			//float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

			inputDirection = new Vector2(x, 0);
			inputDirection = (inputDirection.magnitude > 1) ? inputDirection.normalized : inputDirection;

			joystickImg.rectTransform.anchoredPosition =
				new Vector2(inputDirection.x * (bgImg.rectTransform.sizeDelta.x * 0.42f),
				inputDirection.y * (bgImg.rectTransform.sizeDelta.y * 0.42f));
		}
	}

	public void OnBeginDrag(PointerEventData ped)
	{
		
	}

	public void OnEndDrag(PointerEventData ped)
	{
		
	}

	public void SetActive(bool cond)
	{
		gameObject.SetActive(cond);
	}

	public void moveTransform(Vector2 pos)
	{
		GetComponent<RectTransform>().transform.position = pos;
	}
}
