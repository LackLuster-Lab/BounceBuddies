using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinCodeInput : MonoBehaviour
{
    [SerializeField] TMP_InputField codeText;

    [SerializeField] Button button0;
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;
    [SerializeField] Button button4;
    [SerializeField] Button button5;
    [SerializeField] Button button6;
    [SerializeField] Button button7;
    [SerializeField] Button button8;
    [SerializeField] Button button9;
    [SerializeField] Button backspacebutton;
    // Start is called before the first frame update
    void Start()
    {
        button0.onClick.AddListener(() => {
            addNumber(0);
        });
		button1.onClick.AddListener(() => {
			addNumber(1);
		}); 
        button2.onClick.AddListener(() => {
			addNumber(2);
		});
		button3.onClick.AddListener(() => {
			addNumber(3);
		});
		button4.onClick.AddListener(() => {
			addNumber(4);
		});
		button5.onClick.AddListener(() => {
			addNumber(5);
		});
		button6.onClick.AddListener(() => {
			addNumber(6);
		});
		button7.onClick.AddListener(() => {
			addNumber(7);
		});
		button8.onClick.AddListener(() => {
			addNumber(8);
		});
		button9.onClick.AddListener(() => {
			addNumber(9);
		});
		backspacebutton.onClick.AddListener(() => {
			backspace();
		});
	}

	// Update is called once per frame
	public void show() {
		gameObject.SetActive(true);
		button1.Select();
	}

	public void hide() {
		gameObject.SetActive(false);
	}

	public void backspace() {
		codeText.text = codeText.text.Substring(0, codeText.text.Length - 1);
	}

    private void addNumber(int number) {
        codeText.text = codeText.text + number;
    }
}
