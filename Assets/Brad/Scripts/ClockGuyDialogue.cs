using UnityEngine;

public class ClockGuyDialogue : MonoBehaviour
{
    [Header("Dialogue Pools")]
    [SerializeField, TextArea]
    private string[] earlyLines =
    {
        "Hey! I was counting!",
        "Rude. Just rude.",
        "We were SO close to zero."
    };

    [SerializeField, TextArea]
    private string[] angryLines =
    {
        "AGAIN?! Are you kidding me?!",
        "I am going to lose it.",
        "You do this on purpose, don't you.",
        "I can't believe this. I CANNOT believe this."
    };

    [SerializeField] private int interruptionsBeforeAngry = 3;

    private int _interruptionCount;

    private LevelTimerManager _timer;

    private void Update()
    {
        if (_timer == null)
            TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (LevelTimerManager.Instance == null)
            return;

        _timer = LevelTimerManager.Instance;
        _timer.StateChanged += HandleStateChanged;
    }

    private void HandleStateChanged(TimerDirection direction)
    {
        if (direction == TimerDirection.CountingUp)
        {
            _interruptionCount++;
            SayRandomLine();
        }
    }

    private void SayRandomLine()
    {
        if (DialoguePopupUI.Instance == null)
            return;

        bool isAngry = _interruptionCount > interruptionsBeforeAngry;
        string[] pool = isAngry ? angryLines : earlyLines;
        
        if (pool.Length == 0)
            pool = isAngry ? earlyLines : angryLines;

        if (pool.Length == 0)
            return;

        string line = pool[Random.Range(0, pool.Length)];
        DialoguePopupUI.Instance.ShowMessage(line);
    }

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.StateChanged -= HandleStateChanged;
    }
}