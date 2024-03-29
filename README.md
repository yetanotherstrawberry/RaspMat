# RaspMat
A .NET Framework 4.8.1 WPF MVVM application allowing you to perform Gaussian elimination on a matrix.
This program enables you to reduce a matrix, multiply it or its rows by a scalar, transpose it and insert identity matrices.

![Program image](/Screenshots/NewMat.png)

## Functionality
### Gaussian elimination
This application enables you to get a row echelon form of any matrix as well as its reduced row echelon form. Just press the right button.

![Saving a matrix](/Screenshots/SaveMat.png)

### Edit any cell and operate on rows
Double-click on a cell to edit its value. Click on a row to select it and the choose the scalar and operation. You can select multiple rows.

![Selection](/Screenshots/SelectCell.png)

### JSON serialization
Click "Save matrix" or "Load matrix" to open system dialog that will allow you to save or load the matrix from a JSON file.
