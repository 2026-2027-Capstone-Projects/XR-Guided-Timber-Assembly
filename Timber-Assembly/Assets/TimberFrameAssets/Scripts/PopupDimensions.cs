using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDimensions : MonoBehaviour
{
	[SerializeField, Min(0.0001f)] private float popupScale = 0.001f;
	[SerializeField, Min(0f)] private float offsetRightOfPart = 0.18f;

	private Transform partTransform;
	private Bounds localBounds;
	private float partWidth;
	private float partHeight;
	private float partLength;
	private GameObject popupObject;
	private PopupUIManager subscribedPopupManager;

	public void Show(Transform part, Bounds meshBounds, Vector3 dimensions)
	{
		SubscribeToPopupClosed();
		partTransform = part;
		localBounds = meshBounds;
		partWidth = Mathf.Abs(dimensions.x);
		partHeight = Mathf.Abs(dimensions.y);
		partLength = Mathf.Abs(dimensions.z); 

		if (popupObject == null)
		{
			CreatePopup();
		}

		TMP_Text text = popupObject.GetComponentInChildren<TMP_Text>();
		text.text = $"W {partWidth:F2} m\nH {partHeight:F2} m\nL {partLength:F2} m";
		UpdatePopupTransform();
		popupObject.SetActive(true);
	}

	private void SubscribeToPopupClosed()
	{
		if (subscribedPopupManager == PopupUIManager.Instance)
		{
			return;
		}

		if (subscribedPopupManager != null)
		{
			subscribedPopupManager.PopupClosed -= Hide;
		}

		subscribedPopupManager = PopupUIManager.Instance;
		if (subscribedPopupManager != null)
		{
			subscribedPopupManager.PopupClosed += Hide;
		}
	}

	private void Hide()
	{
		if (popupObject != null)
		{
			popupObject.SetActive(false);
		}
	}

	private void LateUpdate()
	{
		if (popupObject != null && popupObject.activeSelf && partTransform != null)
		{
			UpdatePopupTransform();
		}
	}

	private void CreatePopup()
	{
		popupObject = new GameObject("Dimensions Popup", typeof(RectTransform), typeof(Canvas));
		Canvas canvas = popupObject.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.sortingOrder = 10;

		RectTransform popupRect = popupObject.GetComponent<RectTransform>();
		popupRect.sizeDelta = new Vector2(240f, 120f);
		popupRect.localScale = Vector3.one * popupScale;

		Image background = popupObject.AddComponent<Image>();
		background.color = new Color(0.08f, 0.1f, 0.12f, 0.9f);
		GameObject textObject = new GameObject("Dimensions Text", typeof(RectTransform), typeof(TextMeshProUGUI));
		textObject.transform.SetParent(popupObject.transform, false);

		RectTransform textRect = textObject.GetComponent<RectTransform>();
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = new Vector2(8f, 6f);
		textRect.offsetMax = new Vector2(-8f, -6f);

		TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
		text.fontSize = 30f;
		text.color = Color.white;
		text.alignment = TextAlignmentOptions.Center;
		text.enableWordWrapping = false;
		text.raycastTarget = false;
	}

	private void UpdatePopupTransform()
	{
		Vector3 meshCenter = partTransform.TransformPoint(localBounds.center);
		popupObject.transform.position = meshCenter + partTransform.right * (partWidth * 0.5f + offsetRightOfPart);

		Camera mainCamera = Camera.main;
		if (mainCamera != null)
		{
			Vector3 directionFromCamera = popupObject.transform.position - mainCamera.transform.position;
			if (directionFromCamera.sqrMagnitude > 0.0001f)
			{
				popupObject.transform.rotation = Quaternion.LookRotation(directionFromCamera, mainCamera.transform.up);
			}
		}
	}

	private void OnDestroy()
	{
		if (subscribedPopupManager != null)
		{
			subscribedPopupManager.PopupClosed -= Hide;
		}

		if (popupObject != null)
		{
			Destroy(popupObject);
		}
	}
}
