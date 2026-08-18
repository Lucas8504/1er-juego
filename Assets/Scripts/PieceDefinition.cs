using UnityEngine;

[CreateAssetMenu(fileName = "NewPiece", menuName = "Game/Piece Definition")]
public class PieceDefinition : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public AnimatorOverrideController animatorOverride;
}
