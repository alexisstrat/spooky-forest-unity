using UnityEngine;

namespace Spooky_Forest.Scripts.Data
{
    [CreateAssetMenu(menuName = "Character/Character Item")]
    public class CharacterItem : ScriptableObject
    {
        public int characterId;
        public GameObject model;
        public string characterName;
        public int cost;
        public AudioClip grunt;
        public ParticleSystem hitParticles;
    }
}
