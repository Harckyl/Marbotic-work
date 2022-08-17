using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Marbotic.Game.Literacy {

  using Core.Animation;
  using Core.Attributes;
  using Core.Extensions;
  using Core.Serialization.Dictionaries.Models;

  public class PuzzleManager : MonoBehaviour {

    static readonly int featuredInTrigger = Animator.StringToHash("featured-in");
    static readonly int featuredOutTrigger = Animator.StringToHash("featured-out");
    static readonly int swapTrigger = Animator.StringToHash("swap");
    static readonly int pieceOnComplete = Animator.StringToHash("piece-oncomplete");
    static readonly int pieceLocked = Animator.StringToHash("piece-locked");

    [SerializeField] EventSystem _eventSystem;
    [Space, SerializeField] Animator _puzzleAnimator;
    [Space, SerializeField] GameObject _reticuleClosingPoint;
    [SerializeField] GameObject _reticule;
    [SerializeField] Transform _featuredPieceRoot;

    [Space, SerializeField] PuzzlePiece[] _pieces;
    [Serializable] class PuzzlePieceToFeaturedPieceMapper : SerializedDictionary<PuzzlePiece, GameObject> { }
    [Space, SerializeField] PuzzlePieceToFeaturedPieceMapper _featuredPiecesPrefabs;

    [Space, SerializeReference] ILevelManager _levelManager;
    LevelManager levelManager => _featuredPiece.GetComponent<LevelManager>();

    GameObject _featuredPiece;

    void Awake() {
      var selectedLevel = _levelManager.selectedLevel;
      if (!selectedLevel.HasValue) { return; }

      var currentPiece = _pieces.FirstOrDefault(p => p.levelRange.IsInRange(selectedLevel.Value));
      if (currentPiece != null) {
        InstantiatePiece(_featuredPiecesPrefabs[currentPiece]);
        Swap();
        _levelManager.selectedLevel = null;
      }
    }

    void OnEnable() {
      foreach (var stateMachineEvent in _puzzleAnimator.GetBehaviours<StateMachineEvents>()) {
        stateMachineEvent.stateEntered += HandleStateEntered;
        stateMachineEvent.stateExited += HandleStateExited;
      }
    }

    void OnDisable() {
      foreach (var stateMachineEvent in _puzzleAnimator.GetBehaviours<StateMachineEvents>()) {
        stateMachineEvent.stateEntered -= HandleStateEntered;
        stateMachineEvent.stateExited -= HandleStateExited;
      }
    }

    public void SelectLevel(int level) => _levelManager.selectedLevel = level;

    public void DeselectLevel() => _levelManager.selectedLevel = null;

    public void InstantiatePiece(GameObject piece) {
      DestroyFeaturedPiece();

      _featuredPiece = Instantiate(piece, _featuredPieceRoot);
      levelManager.onStartAnimation += HandleAnimationStarted;
      levelManager.onCompleteAnimation += HandleAnimationFinished;

      var manager =_featuredPiece.GetComponent<LevelManager>();
      manager.levelManager = _levelManager;
      manager.ResetButton();

      SetupReticule();
    }

    public void SetFeaturedOutTrigger() => _puzzleAnimator.SetTrigger(featuredOutTrigger);

    public void SetupReticule() {
      foreach (var button in levelManager.levelButtons.Select(l => l.GetComponent<LevelButton>())) {
        button.SetupReticule(_reticuleClosingPoint, _reticule);
      }
    }

    public void SetFeaturedPiece(PuzzlePiece piece) {
      if ((piece != null) && _featuredPiecesPrefabs.TryGetValue(piece, out var featuredPiecePrefab)) {
        InstantiatePiece(featuredPiecePrefab);
        _puzzleAnimator.SetTrigger(featuredInTrigger);
      } else {
        _puzzleAnimator.SetTrigger(featuredOutTrigger);
      }
    }

    public void UnsetFeaturedPiece() => SetFeaturedPiece(null);

    IEnumerator InstantiateAfterFeaturedOut(int pieceID) {
      yield return new WaitForSeconds(2f);
      _pieces[pieceID].GetComponent<Animator>().SetTrigger(pieceOnComplete);
    }

    void DestroyFeaturedPiece() {
      if (_featuredPiece == null) { return; }
      levelManager.onStartAnimation -= HandleAnimationStarted;
      levelManager.onCompleteAnimation -= HandleAnimationFinished;
      Destroy(_featuredPiece);
    }

    void ResetInstantiatedPiece() => _featuredPiece.GetComponent<LevelManager>().EnableButton();

    void Swap() => _puzzleAnimator.SetTrigger(swapTrigger);

    void HandleStateEntered(Animator animator) => _eventSystem.enabled = true;

    void HandleStateExited(Animator animator) => _eventSystem.enabled = false;

    void HandleAnimationStarted() => _eventSystem.enabled = false;

    void HandleAnimationFinished(int pieceID) {
      SetFeaturedOutTrigger();
      _pieces[pieceID].GetComponent<Animator>().SetTrigger(pieceLocked);
      StartCoroutine(InstantiateAfterFeaturedOut(pieceID));
    }
  }
}
