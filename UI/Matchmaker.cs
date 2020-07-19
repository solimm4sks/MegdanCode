using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Matchmaker : MonoBehaviourPunCallbacks
{
#pragma warning disable 649

    [SerializeField] private string sceneToLoad;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private TMP_Dropdown classDropdown;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private GameObject selectingClassMenu;
    [SerializeField] private GameObject findingMatchMenu;
    [SerializeField] private Button findMatchButton;

    private const int MaxPlayers = 2;
    private const string PPUsernameKey = "username";
    
    private string pClass;
    private string username;

    private bool isConnecting = false;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        pClass = "Knight";
        selectingClassMenu.SetActive(true);
        findingMatchMenu.SetActive(false);
        classDropdown.interactable = true;
        LoadUsername();
        SetButtonState();
    }

    public void FindMatch()
    {
        bool haveClassSave = File.Exists(Application.dataPath + "/SaveData/" + pClass + "DeckSave.txt");
        if (!haveClassSave) {
            errorMessage.text = "Error: You dont have a deck saved for this class. Build a deck.";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        if (!DeckIsValid()) {
            errorMessage.text = "Error when saving deck, please try to build it again..";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        //check for internet connection?
        selectingClassMenu.SetActive(false);
        findingMatchMenu.SetActive(true);
        classDropdown.interactable = false;
        nameField.interactable = false;
        errorMessage.gameObject.SetActive(false);

        SaveCurrentDeck();

        isConnecting = true;
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        }
        else {
            PhotonNetwork.GameVersion = GameInfo.GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        if (isConnecting) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        CancelFinding();
        Debug.Log($"Disconnected bcuz: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Noone is finding a match. Creating room..");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayers });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room :D");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MaxPlayers) {
            Debug.Log("Waiting for opponent");
        }
        else {
            Debug.Log("Match is ready.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount == MaxPlayers) {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Debug.Log("Match is ready.");

            PhotonNetwork.LoadLevel(sceneToLoad);
        }
    }

    private void SetButtonState() {
        findMatchButton.interactable = !string.IsNullOrEmpty(username);
    }

    public void SetClass(int x) {
        pClass = GameInfo.classes[x];
    }

    public void SetUsername(string x) {
        username = x;
        PlayerPrefs.SetString(PPUsernameKey, username);
        PhotonNetwork.NickName = username;

        SetButtonState();
    }

    public void CancelFinding() {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LeaveRoom();
        isConnecting = false;

        selectingClassMenu.SetActive(true);
        findingMatchMenu.SetActive(false);
        classDropdown.interactable = true;
        nameField.interactable = true;
    }

    private void SaveCurrentDeck() {
        string json;
        json = File.ReadAllText(Application.dataPath + "/SaveData/" + pClass + "DeckSave.txt");
        File.WriteAllText(Application.dataPath + "/SaveData/CurrentDeck.txt", json);
    }

    private void LoadUsername() {
        if (PlayerPrefs.HasKey(PPUsernameKey)) {
            username = PlayerPrefs.GetString(PPUsernameKey);
            nameField.text = username;
        }
    }

    private bool DeckIsValid()
    {
        string json;
        json = File.ReadAllText(Application.dataPath + "/SaveData/" + pClass + "DeckSave.txt");
        DeckInfo deckInfo = JsonUtility.FromJson<DeckInfo>(json);
        List<Card> deck = deckInfo.deck;

        if (deck.Count != GameInfo.deckSize)
            return false;
        //probably enough for now..
        return true;
    }
}
