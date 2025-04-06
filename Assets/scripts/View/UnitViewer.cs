using System.Collections;
using UnityEngine;

public class UnitViewer : MonoBehaviour
{
    private const string Runned = "runned";
    private const string Looted = "looted";

    [SerializeField] private Animator _animator;
    [SerializeField] private Unit _unit;

    private int _runned = Animator.StringToHash(Runned);
    private int _looted = Animator.StringToHash(Looted);

    private WaitForSeconds _wait = new WaitForSeconds(0.5f);

    private void OnEnable()
    {
        _unit.Runned += AnimateRun;
        _unit.Looted += AnimateLooting;
        _unit.Stopped += AnimateIdle;
    }

    private void OnDisable()
    {
        _unit.Runned -= AnimateRun;
        _unit.Looted -= AnimateLooting;
        _unit.Stopped -= AnimateIdle;
    }

    private void AnimateRun()
    {
        _animator.SetBool(_runned, true);
        _animator.SetBool(_looted, false);
    }

    private void AnimateLooting()
    {
        StartCoroutine(LootingState());
    }

    private void AnimateIdle()
    {
        _animator.SetBool(_runned, false);
        _animator.SetBool(_looted, false);
    }

    private IEnumerator LootingState()
    {
        _animator.SetBool(_looted, true);

        yield return _wait;

        _animator.SetBool(_looted, false);
    }
}