using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

public class PanelManager : MonoBehaviour {

	public Animator initiallyOpen;

	private int m_OpenParameterId;
	private Animator m_Open;
	private GameObject m_PreviouslySelected;
	
	public bool closeMenuOnCancel = false;
	
	public MenuToggle menuToggle;

	const string k_OpenTransitionName = "Open";
	const string k_ClosedStateName = "Closed";

	public bool focused = false;
	public bool Focused { get => focused; set => focused = value; }

	public UnityEvent OnClosed;

	private void Update()
	{
		var uiModule = (InputSystemUIInputModule) EventSystem.current.currentInputModule;
		if (uiModule.cancel.action.WasPressedThisFrame() && Focused)
		{
			if (m_Open != null)
			{
				CloseCurrent();
				
				if (closeMenuOnCancel)
					menuToggle.Toggle();
			}
		} 
	}

	public void OnEnable()
	{
		m_OpenParameterId = Animator.StringToHash (k_OpenTransitionName);

		if (initiallyOpen == null)
			return;

		OpenPanel(initiallyOpen);
	}

	public void OnMenuToggle()
	{
		if (menuToggle.toggle.isOn)
			OpenPanel(initiallyOpen);
		else
			CloseCurrent();
	}

	public void OpenPanel (Animator anim)
	{
		if (m_Open == anim)
			return;

		anim.gameObject.SetActive(true);
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

		anim.transform.SetAsLastSibling();

		CloseCurrent();

		m_PreviouslySelected = newPreviouslySelected;

		m_Open = anim;
		m_Open.SetBool(m_OpenParameterId, true);

		GameObject go = FindFirstEnabledSelectable(anim.gameObject);

		SetSelected(go);
		
		focused = true;
	}

	static GameObject FindFirstEnabledSelectable (GameObject gameObject)
	{
		GameObject go = null;
		var selectables = gameObject.GetComponentsInChildren<Selectable> (true);
		foreach (var selectable in selectables) {
			if (selectable.IsActive () && selectable.IsInteractable ()) {
				go = selectable.gameObject;
				break;
			}
		}
		return go;
	}

	public void CloseCurrent()
	{
		if (m_Open == null)
			return;

		m_Open.SetBool(m_OpenParameterId, false);
		SetSelected(m_PreviouslySelected);
		StartCoroutine(DisablePanelDeleyed(m_Open));
		m_Open = null;

		OnClosed.Invoke();
		focused = false;
	}

	IEnumerator DisablePanelDeleyed(Animator anim)
	{
		bool closedStateReached = false;
		bool wantToClose = true;
		while (!closedStateReached && wantToClose)
		{
			if (!anim.IsInTransition(0))
				closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

			wantToClose = !anim.GetBool(m_OpenParameterId);

			yield return new WaitForEndOfFrame();
		}

		if (wantToClose)
			anim.gameObject.SetActive(false);
	}

	private void SetSelected(GameObject go) => EventSystem.current.SetSelectedGameObject(go);
}
