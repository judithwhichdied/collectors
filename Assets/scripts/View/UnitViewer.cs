using System.Collections;
using UnityEngine;

public class UnitViewer : MonoBehaviour
{
    private const string Runned = "runned";
    private const string Looted = "looted";

    [SerializeField] private Animator _animator;
    [SerializeField] private Unit _unit;

    private float _delay = 0.5f;

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
        _animator.SetBool(Runned, true);
        _animator.SetBool(Looted, false);
    }

    private void AnimateLooting()
    {
        StartCoroutine(LootingState());
    }

    private void AnimateIdle()
    {
        _animator.SetBool(Runned, false);
        _animator.SetBool(Looted, false);
    }

    private IEnumerator LootingState()
    {
        _animator.SetBool(Looted, true);

        yield return new WaitForSeconds(_delay);

        _animator.SetBool(Looted, false);
    }
}