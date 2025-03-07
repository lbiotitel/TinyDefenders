using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Chatting : NetworkBehaviour
{
    [SerializeField] private GameObject chattingObject;
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private GameObject chattingNodePrefab;
    [SerializeField] private GameObject chattingBox;

    private void Start()
    {
        chatInputField.onEndEdit.AddListener(ParsingChatting);
        chatInputField.onSelect.AddListener(ShowChattingBox);
        chatInputField.onDeselect.AddListener(HideChattingBox);
    }


    private void ShowChattingBox(string input) => chattingBox.SetActive(true);
    private void HideChattingBox(string input) => chattingBox.SetActive(false);

    private void ParsingChatting(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        chatInputField.text = string.Empty;
        chatInputField.ActivateInputField();
        SendChatToServerRpc(input);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatToServerRpc(string text)
    {
        SendChatToClientRpc(text);
    }

    [ClientRpc]
    private void SendChatToClientRpc(string text)
    {
        GameObject go = Instantiate(chattingNodePrefab);
        go.GetComponent<TextMeshProUGUI> ().text = text;
        go.transform.SetParent(chattingObject.transform);
        go.transform.SetAsFirstSibling();
    }
}
