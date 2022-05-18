using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;

    private int currentPlayerIndex = -1;

    void Update()
    {
        var switchForward = Input.GetButtonDown("SwitchForward");
        var switchBackward = Input.GetButtonDown("SwitchBackward");

        if (switchForward || switchBackward)
        {
            var switchDirection = switchForward ? 1 : -1;

            if (currentPlayerIndex > -1)
            {
                var currentPlayer = players[currentPlayerIndex];
                currentPlayer.SetActivePlayer(false);
            }

            currentPlayerIndex += Mathf.RoundToInt(Mathf.Sign(switchDirection));

            if (currentPlayerIndex < 0)
                currentPlayerIndex = players.Length - 1;
            else if (currentPlayerIndex >= players.Length)
                currentPlayerIndex = 0;

            var newCurrentPlayer = players[currentPlayerIndex];
            newCurrentPlayer.SetActivePlayer(true);
        }
    }
}
