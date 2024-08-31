using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class DataFetcher : MonoBehaviour
{
    public Material cellMaterial; // Assign a material in the Unity Inspector
    public Color headerColor = Color.cyan; // Color for the first row cells

    void Start()
    {
        SetupCamera();
        SetupLighting();
        Debug.Log("Setup complete. Fetching data...");
        StartCoroutine(FetchData());
    }

    IEnumerator FetchData()
    {
        // Replace with your EtherCalc CSV URL
        string url = "https://ethercalc.net/h29023k3e0.csv";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch data: " + www.error);
            }
            else
            {
                Debug.Log("Data fetched successfully.");
                string csvData = www.downloadHandler.text;
                ParseCSV(csvData);
            }
        }
    }

    void ParseCSV(string csvData)
    {
        Debug.Log("Parsing CSV data...");
        string[] rows = csvData.Split('\n');
        List<string[]> table = new List<string[]>();

        // Limit to the first 20 rows for simplicity
        int rowLimit = Mathf.Min(rows.Length, 20);

        for (int i = 0; i < rowLimit; i++)
        {
            string[] columns = rows[i].Split(',');
            table.Add(columns);
        }

        Create3DTable(table.ToArray());
    }

    void Create3DTable(string[][] table)
    {
        Debug.Log("Creating 3D table...");

        // Calculate the screen width and height
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2.0f;
        float screenHeight = Camera.main.orthographicSize * 2.0f;

        // Define the table width and height as 80% of the screen dimensions
        float tableWidth = screenWidth * 0.8f;
        float tableHeight = screenHeight * 0.8f;

        // Calculate cell dimensions
        float cellWidth = tableWidth / table[0].Length; // Width of each cell
        float cellHeight = tableHeight / table.Length;  // Height of each cell

        // Increase table width and adjust starting position
        float startX = -tableWidth / 2.0f; // Center the table horizontally
        float startY = tableHeight / 2.0f; // Center the table vertically

        for (int row = 0; row < table.Length; row++)
        {
            for (int col = 0; col < table[row].Length; col++)
            {
                Vector3 position = new Vector3(startX + col * cellWidth, startY - row * cellHeight, 0);
                Debug.Log($"Creating cell at position: {position} with content: {table[row][col]}");
                // Pass whether the cell is in the first row or not
                bool isHeader = row == 0;
                CreateCell(position, table[row][col], cellWidth, cellHeight, isHeader);
            }
        }
    }

    void CreateCell(Vector3 position, string content, float cellWidth, float cellHeight, bool isHeader)
    {
        Debug.Log($"Creating cell at position: {position} with content: {content}");

        // Create a cuboid cell with increased width and height
        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.transform.position = position;
        cell.transform.localScale = new Vector3(cellWidth, cellHeight, 0.1f); // Adjust width, height, and thickness

        // Apply color based on whether the cell is in the header row
        if (isHeader)
        {
            cell.GetComponent<Renderer>().material.color = headerColor;
        }
        else
        {
            cell.GetComponent<Renderer>().material = cellMaterial; // Apply the material for non-header cells
        }

        // Add text to the cell
        GameObject textObj = new GameObject("CellText");
        textObj.transform.position = position; // Center text in the cell
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = content;
        textMesh.fontSize = 24; // Adjust font size to prevent overlap
        textMesh.characterSize = 0.1f; // Adjust character size for better visibility
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter; // Center text both horizontally and vertically
        textMesh.color = Color.white; // Ensure text is visible against cell color

        // Slightly adjust text position to avoid clipping with the cell
        textObj.transform.localPosition += new Vector3(0, 0, -0.05f); // Move text slightly forward on the Z-axis
    }

    void SetupCamera()
    {
        // Calculate the screen width and height
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2.0f;
        float screenHeight = Camera.main.orthographicSize * 2.0f;

        // Define the table width and height as 80% of the screen dimensions
        float tableWidth = screenWidth * 0.8f;
        float tableHeight = screenHeight * 0.8f;

        // Set camera position to fit the table properly
        Camera.main.orthographicSize = tableHeight / 2.0f * 1.2f; // Adjust zoom level
        Camera.main.transform.position = new Vector3(0, 0, -10); // Adjust based on zoom level
        Camera.main.transform.LookAt(Vector3.zero); // Make camera look at the center of the table
        Debug.Log("Camera setup complete.");
    }

    void SetupLighting()
    {
        GameObject lightObj = new GameObject("DirectionalLight");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.position = new Vector3(0, 10, 10); // Position the light
        light.transform.LookAt(Vector3.zero); // Point the light towards the center of the table
        Debug.Log("Lighting setup complete.");
    }
}
