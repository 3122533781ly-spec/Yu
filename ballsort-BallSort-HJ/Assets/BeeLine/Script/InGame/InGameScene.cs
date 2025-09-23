using UnityEngine;

public class InGameScene : MonoBehaviour
{
    private void Start()
    {
        _inGame.StateModel.CurrentState = InLineBeeGameState.Standby;
    }

    [SerializeField] private InGameLineBee _inGame;
}