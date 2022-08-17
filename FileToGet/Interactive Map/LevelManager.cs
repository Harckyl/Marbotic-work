using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

namespace Marbotic.Game.Literacy {

  using Core.Attributes;
  using Holders.Models;

  public class LevelManager : MonoBehaviour {

    static readonly int _levelComplete = Animator.StringToHash("level-complete");
    static readonly int _levelFinished = Animator.StringToHash("level-finished");
    static readonly int _levelCurrent = Animator.StringToHash("level-current");
    static readonly int _pieceComplete = Animator.StringToHash("piece-complete");

    [SerializeField] int _pieceID;
    [SerializeField] float _timeToWait;

    [SerializeField] GameObject[] _levelButtons;
    public GameObject[] levelButtons => _levelButtons;

    Coroutine _coroutine;
    bool _wait = false;
    public event Action<int> onCompleteAnimation;
    public event Action onStartAnimation;

    ILevelManager _levelManager;
    public ILevelManager levelManager {
      get => _levelManager;
      set {
        _levelManager = value;
        foreach(var levelButton in _levelButtons) {
          levelButton.GetComponent<LevelButton>().levelManager = value;
        }
      }
    }

    public void DisableButton() {
      foreach (var button in _levelButtons) {
        button.GetComponent<Button>().interactable = false;
      }
    }

    public void EnableButton() {
      foreach (var button in _levelButtons) {
        button.GetComponent<Button>().interactable = true;
      }
    }

    public void ResetButton() {
      int progression = levelManager.currentScaffoldingLevelIndex;
      GameObject waitingButton = null;
      foreach (var Button in _levelButtons) {
        var levelButton = Button.GetComponent<LevelButton>();
        levelButton.SetupIcon();
        if (progression < levelButton.level) {
          //do nothing
        } else if (progression == levelButton.level) {
          waitingButton = Button;
        } else {
          if (levelManager.selectedLevel == levelButton.level && levelManager.activitySuccess) {
            _wait = true;
            StartCoroutine(SetComplete(levelButton.GetComponent<Animator>()));
          } else {
            levelButton.GetComponent<Animator>().SetTrigger(_levelFinished);
          }
        }
      }

      if (_wait && waitingButton != null) {
        _coroutine = StartCoroutine(ExecuteAfterTime(_timeToWait, waitingButton));
      } else if (waitingButton != null) {
        waitingButton.GetComponent<Animator>().SetTrigger(_levelCurrent);
      } else if (waitingButton == null && _wait) {
        _coroutine = StartCoroutine(ExecuteAfterTime(_timeToWait, null));
      }
    }

    // FIXME: to add a delay, which makes it wait for the reticule to take its time to open
    IEnumerator SetComplete(Animator level) {
      yield return new WaitForSeconds(1);
      level.SetTrigger(_levelComplete);
    }

    IEnumerator ExecuteAfterTime(float time, GameObject button) {
      yield return new WaitForSeconds(time);
      if (button != null) {
        button.GetComponent<Animator>().SetTrigger(_levelCurrent);
      } else {
        if (levelManager.incrementsScaffoldingLevel) {
          _coroutine = StartCoroutine(OnCompleteAnimation());
        }
      }
    }

    IEnumerator OnCompleteAnimation() {
      onStartAnimation.Invoke();
      foreach (var button in _levelButtons) {
        button.GetComponent<Animator>().SetTrigger(_pieceComplete);
      }
      yield return new WaitForSeconds(2f);
      onCompleteAnimation.Invoke(_pieceID);
    }
  }
}
