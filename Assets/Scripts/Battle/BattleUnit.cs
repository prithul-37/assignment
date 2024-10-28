
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] Image pokemonSprite;
    Vector3 originalPos;
    Color originalColor;

    public Pokemon Pokemon { get; set; }

    private void Awake()
    {
        originalPos = pokemonSprite.transform.localPosition;
        originalColor = pokemonSprite.color;
    }
    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit)
            pokemonSprite.sprite = Pokemon.Base.BackSprite;
        else
            pokemonSprite.sprite = Pokemon.Base.ForntSprite;
        
        pokemonSprite.color = originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            pokemonSprite.transform.localPosition = new Vector3(-500, originalPos.y);
        else
            pokemonSprite.transform.localPosition = new Vector3(500, originalPos.y);
        
        pokemonSprite.transform.DOLocalMoveX(originalPos.x,1f);
    }

    public void PlayAttactAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPos.x + 50f, .25f));
        else
            sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPos.x - 50f, .25f));

        sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPos.x, .25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonSprite.DOColor(Color.gray, .1f));
        sequence.Append(pokemonSprite.DOColor(originalColor, .1f));
    }

    public void PlayFaintedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonSprite.transform.DOLocalMoveY(originalPos.y - 150f,0.5f));
        sequence.Join(pokemonSprite.DOFade(0f,.5f));
    }
}