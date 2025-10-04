using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nickNameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registrateButton;
    [SerializeField] private Button guestButton;

    [SerializeField] private Button completeButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private AuthUIManager authUIManager;
    private LoginOrRegistrate loginOrRegistrate;

    private void Start()
    {
        SetOnClickEventToButtons();
        OnReturnButtonClick();
    }
    private enum LoginOrRegistrate
    {
        login,
        registrate
    }
    private void OnLoginButtonClick()
    {
        emailField.gameObject.SetActive(true);
        passwordField.gameObject.SetActive(true);
        completeButton.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
        loginOrRegistrate = LoginOrRegistrate.login;

        loginButton.gameObject.SetActive(false);
        registrateButton.gameObject.SetActive(false);
        guestButton.gameObject.SetActive(false);
    }
    private void OnRegisterButtonClick()
    {
        emailField.gameObject.SetActive(true);
        passwordField.gameObject.SetActive(true);
        nickNameField.gameObject.SetActive(true);

        completeButton.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
        loginOrRegistrate = LoginOrRegistrate.registrate;

        loginButton.gameObject.SetActive(false);
        registrateButton.gameObject.SetActive(false);
        guestButton.gameObject.SetActive(false);
    }

    private void OnReturnButtonClick()
    {
        loginButton.gameObject.SetActive(true);
        registrateButton.gameObject.SetActive(true);
        guestButton.gameObject.SetActive(true);

        completeButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);

        emailField.gameObject.SetActive(false);
        passwordField.gameObject.SetActive(false);
        nickNameField.gameObject.SetActive(false);

        emailField.text="";
        passwordField.text = "";
        nickNameField.text = "";

        completeButton.gameObject.SetActive(false);
    }
    private void OnCompleteButtonClick()
    {
        if (loginOrRegistrate == LoginOrRegistrate.login)
        {
            authUIManager.OnLoginButtonClick(emailField.text,passwordField.text);
        }
        else if (loginOrRegistrate == LoginOrRegistrate.registrate)
        {
            authUIManager.OnRegisterButtonClick(emailField.text,passwordField.text,nickNameField.text);
        }
    }
    private void SetOnClickEventToButtons()
    {
        guestButton.onClick.AddListener(() =>
        {

        });
        loginButton.onClick.AddListener(() =>
        {
            OnLoginButtonClick();
        });
        registrateButton.onClick.AddListener(() =>
        {
            OnRegisterButtonClick();
        });
        returnButton.onClick.AddListener(() =>
        {
            OnReturnButtonClick();
        });
        completeButton.onClick.AddListener(() =>
        {
            OnCompleteButtonClick();
        });
        
    }
}
