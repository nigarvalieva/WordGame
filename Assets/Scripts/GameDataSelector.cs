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
        // ���������, ���� �� ������ � levelData
        if (levelData == null || levelData.data == null || levelData.data.Count == 0)
        {
            Debug.LogError("LevelData ��� ��� ������ ������ �� ���������������.");
            return;
        }

        foreach (var data in levelData.data)
        {
            if (data.categoryName == CurrentGameData.selectedCategoryName)
            {
                if (data.boardData == null || data.boardData.Count == 0)
                {
                    Debug.LogError($"BoardData ���� ��� ���������: {data.categoryName}");
                    return;
                }

                // �������� ������, ���������� ���������, ���� ������ �����������
                var boardIndex = DataSave.ReadCategoryCurrentIndexValues(CurrentGameData.selectedCategoryName);

                // ���� ������ ������ ����, ��� ������ ��� ����� ������� ������, ���������� ��������� ������
                if (boardIndex < 0 || boardIndex >= data.boardData.Count)
                {
                    Debug.LogWarning($"������ {boardIndex} ��� ���������. ���������� ��������� ������.");
                    boardIndex = Random.Range(0, data.boardData.Count); // ��������� ������ �� 0 �� (size-1)
                }

                // ����������� ������ �� ������ ���������� ��� ���������� �������
                CurrentGameData.selectedBoardData = data.boardData[boardIndex];

                // ��������� ����, ���� ����� ������ ���������
                return;
            }
        }

        Debug.LogError("�� ������� ���������, ��������������� ���������: " + CurrentGameData.selectedCategoryName);
    }
}
