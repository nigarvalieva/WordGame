using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsGrid : MonoBehaviour
{
    public GameData currentGameData;
    public GameObject gridSquarePrefab;
    public AlphabetData alphabetData;

    public float squareOffset = 0.0f;
    public float topPosition;

    private List<GameObject> _squareList = new List<GameObject>();


    void Start()
    {
        if (currentGameData == null)
        {
            Debug.LogError("currentGameData не назначен.");
            return;
        }

        if (alphabetData == null)
        {
            Debug.LogError("alphabetData не назначен.");
            return;
        }

        if (gridSquarePrefab == null)
        {
            Debug.LogError("gridSquarePrefab не назначен.");
            return;
        }

        SpawnGridSquares();
        SetSquaresPosition();
    }

    private void SetSquaresPosition()
    {
        if (_squareList.Count == 0) return;

        var squareRect = _squareList[0].GetComponent<SpriteRenderer>().sprite.rect;
        var squareTransform = _squareList[0].GetComponent<Transform>();

        var offset = new Vector2
        {
            x = (squareRect.width * squareTransform.localScale.x + squareOffset) * 0.01f,
            y = (squareRect.height * squareTransform.localScale.y + squareOffset) * 0.01f
        };

        var startPosition = GetFirstSquarePosition();
        int columnNumber = 0;
        int rowNumber = 0;

        foreach (var square in _squareList)
        {
            if (rowNumber + 1 > currentGameData.selectedBoardData.Rows)
            {
                columnNumber++;
                rowNumber = 0;
            }

            var positionX = startPosition.x + offset.x * columnNumber;
            var positionY = startPosition.y - offset.y * rowNumber;

            square.GetComponent<Transform>().position = new Vector2(positionX, positionY);
            rowNumber++;
        }
    }


    private Vector2 GetFirstSquarePosition()
    {
        var startPosition = new Vector2(0f, transform.position.y);
        var squareRect = _squareList[0].GetComponent<SpriteRenderer>().sprite.rect;
        var squareTransform = _squareList[0].GetComponent<Transform>();
        var squareSize = new Vector2(0f, 0f);

        squareSize.x = squareRect.width * squareTransform.localScale.x;
        squareSize.y = squareRect.height * squareTransform.localScale.y;

        var midWidthPosition = (((currentGameData.selectedBoardData.Columns - 1) * squareSize.x) / 2) * 0.01f;
        var midWidthHeight = (((currentGameData.selectedBoardData.Rows - 1) * squareSize.y) / 2) * 0.01f;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y += midWidthHeight;

        return startPosition;
    }



    private void SpawnGridSquares()
    {
        if (currentGameData == null)
        {
            Debug.LogError("currentGameData не назначен.");
            return;
        }

        if (alphabetData == null)
        {
            Debug.LogError("alphabetData не назначен.");
            return;
        }

        var squareScale = GetSquareScale(new Vector3(1.5f, 1.5f, 0.1f));
        foreach (var squares in currentGameData.selectedBoardData.Board)
        {
            foreach (var squareLetter in squares.Row)
            {
                var normalLetterData = alphabetData.AlphabetNormal.Find(data => data.letter == squareLetter);
                var selectedLetterData = alphabetData.AlphabetHighlighted.Find(data => data.letter == squareLetter);
                var correctLetterData = alphabetData.AlphabetWrong.Find(data => data.letter == squareLetter);

                // Добавлены проверки на null для данных
                if (normalLetterData == null || selectedLetterData == null || correctLetterData == null)
                {
                    Debug.LogError($"Не удалось найти данные для буквы: {squareLetter}");
                    continue;
                }

                if (normalLetterData.image == null || selectedLetterData.image == null)
                {
                    Debug.LogError("Все поля в вашем массиве должны содержать буквы. " +
                        "Нажмите кнопку 'Заполнить случайными' в данных доски для добавления случайной буквы. Буква: " + squareLetter);

#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
                        UnityEditor.EditorApplication.isPlaying = false;
                    }
#endif
                }
                else
                {
                    _squareList.Add(Instantiate(gridSquarePrefab));
                    var square = _squareList[_squareList.Count - 1];
                    var gridSquare = square.GetComponent<GridSquare>();

                    // Убедитесь, что компонент GridSquare существует
                    if (gridSquare != null)
                    {
                        gridSquare.SetSprite(normalLetterData, correctLetterData, selectedLetterData);
                        square.transform.SetParent(this.transform);
                        square.GetComponent<Transform>().position = new Vector3(0f, 0f, 0f);
                        square.transform.localScale = squareScale;
                        gridSquare.SetIndex(_squareList.Count - 1);
                    }
                    else
                    {
                        Debug.LogError("Компонент GridSquare не найден на prefab.");
                    }
                }
            }
        }
    }

    private Vector3 GetSquareScale(Vector3 defaultScale)
    {
        var finalScale = defaultScale;
        var adjustment = 0.01f;

        while (ShouldScaleDown(finalScale))
        {
            finalScale.x -= adjustment;
            finalScale.y -= adjustment;

            if (finalScale.x <= 0 || finalScale.y <= 0)
            {
                finalScale.x = adjustment;
                finalScale.y = adjustment;
                return finalScale;
            }
        }
        return finalScale;
    }

    private bool ShouldScaleDown(Vector3 targetScale)
    {
        var squareRect = gridSquarePrefab.GetComponent<SpriteRenderer>().sprite.rect;
        var squareSize = new Vector2(0f, 0f);
        var startPosition = new Vector2(0f, 0f);

        squareSize.x = (squareRect.width + targetScale.x) + squareOffset;
        squareSize.y = (squareRect.height + targetScale.y) + squareOffset;

        var midWidthPosition = ((currentGameData.selectedBoardData.Columns * squareSize.x) / 2) * 0.01f;
        var midWidthHeight = ((currentGameData.selectedBoardData.Rows * squareSize.y) / 2) * 0.01f;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y = midWidthHeight;

        return startPosition.x < GetHalfScreenWidth() * -1 || startPosition.y > topPosition;
    }

    private float GetHalfScreenWidth()
    {
        float height = Camera.main.orthographicSize * 2;
        float width = (1.7f * height) * Screen.width / Screen.height;
        return width / 2;
    }
}
