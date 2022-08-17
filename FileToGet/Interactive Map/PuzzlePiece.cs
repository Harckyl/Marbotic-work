using UnityEngine;

namespace Marbotic.Game.Literacy {

  using Core.Extensions;
  using Holders.Models;

  public class PuzzlePiece : MonoBehaviour {

    static int _pieceBlinking = Animator.StringToHash("piece-current");
    static int _pieceShown = Animator.StringToHash("piece-finished");

    [SerializeField] int _minLevel;
    [SerializeField] int _maxLevel;
    public Vector2Int levelRange => new Vector2Int { x = _minLevel, y = _maxLevel };

    [SerializeReference] ILevelManager _levelManager;
    [SerializeField] Animator _animator;

    void OnEnable() => ResetPiece();

    void OnValidate() => _minLevel = Mathf.Min(_minLevel, _maxLevel);

    public void ResetPiece() {
      var progression = _levelManager.currentScaffoldingLevelIndex;
      if (levelRange.IsInRange(progression)) {
        _animator.SetTrigger(_pieceBlinking);
      } else if (levelRange.IsAboveMax(progression)) {
        _animator.SetTrigger(_pieceShown);
      }
    }
  }
}
