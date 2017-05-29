using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Card : MonoBehaviour, IPointerClickHandler
{
    public float fadeSpeed = 3;

    public int Id { get; private set; }

    public event EventHandler<EventArgs> OnCardClick;

    [SerializeField]
    private GameObject back;

    private Image image;
    private Animator animator;

    void Awake() {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    public void Reveal() {
        back.SetActive(false);
    }

    public void UnReveal() {
        back.SetActive(true);
    }

    public void Shake() {
        if (animator != null) {
            animator.SetTrigger("shake");
        }
    }

    public IEnumerator Fade() {
        while (image.color.a >= 0.01f) {
            image.color = new Color(1, 1, 1, Mathf.Lerp(image.color.a, 0, Time.deltaTime * fadeSpeed));
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetImage(int id, Sprite sprite) {
        image.sprite = sprite;
        Id = id;

        StopCoroutine(Fade());
        image.color = new Color(1, 1, 1, 1);
        back.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (back.activeSelf && OnCardClick != null) {
            OnCardClick(this, null);
        }
    }
}
