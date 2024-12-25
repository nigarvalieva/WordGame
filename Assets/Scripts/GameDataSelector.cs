using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataSelector : MonoBehaviour
{
    public GameData CurrentGameData;
    public GameLevelData levelData;

    void Awake()
    {
        SelectSequentalBoardData();
    }

    private void SelectSequentalBoardData()
    {
        // ѕровер€ем, есть ли данные в levelData
        if (levelData == null || levelData.data == null || levelData.data.Count == 0)
        {
            Debug.LogError("LevelData или его список данных не инициализирован.");
            return;
        }

        foreach (var data in levelData.data)
        {
            if (data.categoryName == CurrentGameData.selectedCategoryName)
            {
                if (data.boardData == null || data.boardData.Count == 0)
                {
                    Debug.LogError($"BoardData пуст дл€ категории: {data.categoryName}");
                    return;
                }

                // ѕолучаем индекс, используем обработку, если индекс некорректен
                var boardIndex = DataSave.ReadCategoryCurrentIndexValues(CurrentGameData.selectedCategoryName);

                // ≈сли индекс меньше нул€, или больше или равен размеру списка, используем случайный индекс
                if (boardIndex < 0 || boardIndex >= data.boardData.Count)
                {
                    Debug.LogWarning($"»ндекс {boardIndex} вне диапазона. »спользуем случайный индекс.");
                    boardIndex = Random.Range(0, data.boardData.Count); // —лучайный индекс от 0 до (size-1)
                }

                // ѕрисваиваем данные на основе выбранного или случайного индекса
                CurrentGameData.selectedBoardData = data.boardData[boardIndex];

                // ѕрерываем цикл, если нашли нужную категорию
                return;
            }
        }

        Debug.LogError("Ќе найдена категори€, соответствующа€ выбранной: " + CurrentGameData.selectedCategoryName);
    }
}
