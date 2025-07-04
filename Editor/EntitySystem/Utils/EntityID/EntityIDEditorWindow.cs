using System.Collections.Generic;
using System.IO;
using SpookyCore.Runtime.EntitySystem;
using UnityEditor;
using UnityEngine;

namespace SpookyCore.Editor.EntitySystem
{
    public class EntityIDEditorWindow : EditorWindow
    {
        #region Fields

        private static string DBPath = "";
        private static string OutputPath = "";

        private EntityIDDatabase _database;
        private Vector2 _scroll;
        private List<bool> _foldouts = new();
        
        //Add Category
        private string _newCategoryName = "";
        private bool _isAddingNewCategory;
        //Add Entity
        private string _newEntityName = "";
        private int _newEntityIndex = -1;

        //Edit Category
        private string _editingCategoryName = "";
        private int _editingCategoryIndex = -1;
        //Edit Entity
        private string _editingEntityName = "";
        private int _editingEntityCategoryIndex = -1;
        private int _editingEntityIndex = -1;

        #endregion

        #region Life Cycle

        [MenuItem("SpookyTools/Entity System/EntityID Editor")]
        public static void ShowWindow()
        {
            GetWindow<EntityIDEditorWindow>("EntityID Editor");
        }

        private void OnEnable()
        {
            if (DBPath == "")
            {
                DBPath = Path.Combine(SpookyPathResolver.GetSpookyCorePath(), "Runtime/EntitySystem/Utils/EntityID/EntityIDDatabase.asset");
            }

            if (OutputPath == "")
            {
                OutputPath = Path.Combine(SpookyPathResolver.GetSpookyCorePath(), "Runtime/EntitySystem/EntityID.cs");
            }
            
            _database = AssetDatabase.LoadAssetAtPath<EntityIDDatabase>(DBPath);
            
            if (!_database)
            {
                _database = CreateInstance<EntityIDDatabase>();
                Directory.CreateDirectory(Path.GetDirectoryName(DBPath) ?? string.Empty);
                AssetDatabase.CreateAsset(_database, DBPath);
                AssetDatabase.SaveAssets();
            }
            
            _foldouts = new List<bool>(_database.Categories.Count);
            for (var i = 0; i < _database.Categories.Count; i++)
            {
                _foldouts.Add(true);
            }
        }

        private void OnGUI()
        {
            if (!_database)
            {
                EditorGUILayout.HelpBox("Could not find or create EntityIDDatabase asset.", MessageType.Error);
                return;
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            for (var i = 0; i < _database.Categories.Count; i++)
            {
                if (_foldouts.Count <= i) _foldouts.Add(true);
                var category = _database.Categories[i];

                EditorGUILayout.BeginVertical("box");
                    //Category Box Label, Edit, Delete
                    EditorGUILayout.BeginHorizontal();
                        _foldouts[i] = EditorGUILayout.Foldout(_foldouts[i], "", true);
                        HandleEditCategory(i);
                        if (HandleDeleteCategory(i))
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            i--;
                            continue;
                        }
                    EditorGUILayout.EndHorizontal();
                    if (!_foldouts[i])
                    {
                        EditorGUILayout.EndVertical();
                        continue;
                    }
                    
                    
                    EditorGUI.indentLevel++;
                    for (var j = 0; j < category.Entries.Count; j++)
                    {
                        //Entry Label, Edit, Delete
                        EditorGUILayout.BeginHorizontal();

                        var entryID = 100 + i * 100 + j + 1;
                        HandleEditEntry(i, j, entryID);

                        if (HandleDeleteEntry(i, j))
                        {
                            EditorGUILayout.EndHorizontal();
                            break;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;

                    //Entry Add
                    HandleAddEntry(i);
                EditorGUILayout.EndVertical();
            }

            //Category Add
            HandleAddCategory();

            EditorGUILayout.Space(10);

            //Generate
            if (GUILayout.Button("Generate EntityID and EntityType"))
            {
                GenerateCode();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Private Methods

        private void HandleAddCategory()
        {
            if (_isAddingNewCategory)
            {
                EditorGUILayout.BeginHorizontal();
                _newCategoryName = EditorGUILayout.TextField(_newCategoryName).Replace(" ", "_");
                if (GUILayout.Button("Add"))
                {
                    if (IsCategoryNameValid(_newCategoryName, out var catID))
                    {
                        _database.Categories.Add(new EntityIDDatabase.EntityIDCategory(_newCategoryName));
                        _newCategoryName = "";
                        EditorUtility.SetDirty(_database);
                        _isAddingNewCategory = false;
                    }
                    else
                    {
                        Debug.LogWarning($"Category {_newCategoryName} exists at entry {catID}. Please choose another Category name.");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else if (GUILayout.Button("Add New Category"))
            {
                _isAddingNewCategory = true;
            }
        }
        private void HandleEditCategory(int categoryIndex)
        {
            var category = _database.Categories[categoryIndex];
            
            if (_editingCategoryIndex == categoryIndex)
            {
                _editingCategoryName = EditorGUILayout.TextField(_editingCategoryName).Replace(" ", "_");
                if (GUILayout.Button("Save", GUILayout.Width(50)))
                {
                    if (IsCategoryNameValid(_editingCategoryName, out var catID))
                    {
                        category.CategoryName = _editingCategoryName;
                        _editingCategoryIndex = -1;
                        EditorUtility.SetDirty(_database);
                    }
                    else
                    {
                        Debug.LogWarning($"Category {_editingCategoryName} exists at entry {catID}. Please choose another Category name.");
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField($"[{100 + categoryIndex * 100}] {category.CategoryName}", EditorStyles.boldLabel);
                if (GUILayout.Button("✎", GUILayout.Width(25)))
                {
                    _editingCategoryIndex = categoryIndex;
                    _editingCategoryName = category.CategoryName;
                }
            }
        }
        private bool HandleDeleteCategory(int categoryIndex)
        {
            var category = _database.Categories[categoryIndex];
            if (GUILayout.Button("✖", GUILayout.Width(25)))
            {
                if (EditorUtility.DisplayDialog("Delete Category", $"Are you sure you want to delete category '{category.CategoryName}'?", "Yes", "Cancel"))
                {
                    _database.Categories.RemoveAt(categoryIndex);
                    _foldouts.RemoveAt(categoryIndex);
                    _editingCategoryIndex = -1;
                    EditorUtility.SetDirty(_database);
                    return true;
                }
            }

            return false;
        }

        private void HandleAddEntry(int categoryIndex)
        {
            if (_newEntityIndex == categoryIndex)
            {
                var category = _database.Categories[categoryIndex];
                EditorGUILayout.BeginHorizontal();
                _newEntityName = EditorGUILayout.TextField(_newEntityName).Replace(" ", "");
                if (GUILayout.Button("Add"))
                {
                    if (IsEntityIDValid(_newEntityName, out var entryID))
                    {
                        category.Entries.Add(_newEntityName);
                        _newEntityName = "";
                        _newEntityIndex = -1;
                        EditorUtility.SetDirty(_database);
                    }
                    else
                    {
                        Debug.LogWarning(entryID == -1
                            ? "EntityID is blank. Please choose another EntityID."
                            : $"EntityID {_newEntityName} exists at entry {entryID}. Please choose another EntityID name.");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else if (GUILayout.Button("Add New EntityID"))
            {
                _newEntityIndex = categoryIndex;
            }
        }
        private void HandleEditEntry(int categoryIndex, int entryIndex, int entityID)
        {
            var category = _database.Categories[categoryIndex];
            
            if (_editingEntityIndex == entryIndex && _editingEntityCategoryIndex == categoryIndex)
            {
                _editingEntityName = EditorGUILayout.TextField(_editingEntityName).Replace(" ", "");
                if (GUILayout.Button("Save", GUILayout.Width(50)))
                {
                    if (IsEntityIDValid(_editingEntityName, out var entryID))
                    {
                        category.Entries[entryIndex] = _editingEntityName;
                        _editingEntityIndex = -1;
                        EditorUtility.SetDirty(_database);
                    }
                    else
                    {
                        Debug.LogWarning($"EntityID {_editingEntityName} exists at entry {entryID}. Please choose another EntityID name.");
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField($"{entityID} - {category.Entries[entryIndex]}");
                if (GUILayout.Button("✎", GUILayout.Width(25)))
                {
                    _editingEntityCategoryIndex = categoryIndex;
                    _editingEntityIndex = entryIndex;
                    _editingEntityName = category.Entries[entryIndex];
                }
            }
        }
        private bool HandleDeleteEntry(int categoryIndex, int entryIndex)
        {
            var category = _database.Categories[categoryIndex];
            if (GUILayout.Button("✖", GUILayout.Width(25)))
            {
                if (EditorUtility.DisplayDialog("Delete Entry", $"Delete entry '{category.Entries[entryIndex]}'?", "Yes", "Cancel"))
                {
                    category.Entries.RemoveAt(entryIndex);
                    _editingEntityIndex = -1;
                    EditorUtility.SetDirty(_database);
                    return true;
                }
            }

            return false;
        }

        private bool IsCategoryNameValid(string categoryName, out int exisingCategoryID)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                exisingCategoryID = -1;
                return false;
            }

            for (var i = 0; i < _database.Categories.Count; ++i)
            {
                var category = _database.Categories[i];
                if (category.CategoryName == categoryName)
                {
                    exisingCategoryID = 100 * (i + 1);
                    return false;
                }
            }

            exisingCategoryID = -1;
            return true;
        }
        private bool IsEntityIDValid(string entityName, out int existEntityID)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                existEntityID = -1;
                return false;
            }

            for (var i = 0; i < _database.Categories.Count; ++i)
            {
                var category = _database.Categories[i];
                for (var j = 0; j < _database.Categories[i].Entries.Count; ++j)
                {
                    var entry = category.Entries[j];
                    if (entry == entityName)
                    {
                        var catID = 100 * (i + 1);
                        existEntityID = catID + j + 1;
                        return false;
                    }
                }
            }

            existEntityID = -1;
            return true;
        }
        
        private void GenerateCode()
        {
            using StreamWriter writer = new(OutputPath);

            writer.WriteLine("//Auto-generated by EntityIDEditorWindow, configurable via SpookyTools/EntitySystem/EntityID Editor.");
            writer.WriteLine("//DO NOT change manually.");
            writer.WriteLine("using System;");
            writer.WriteLine();
            writer.WriteLine("namespace SpookyCore.Runtime.EntitySystem");
            writer.WriteLine("{");
            writer.WriteLine("    [Serializable]");
            writer.WriteLine("    public enum EntityID");
            writer.WriteLine("    {");
            writer.WriteLine("        MISSING_ID = 0,");

            for (var i = 0; i < _database.Categories.Count; i++)
            {
                var baseID = 100 + i * 100;
                var cat = _database.Categories[i];
                writer.WriteLine($"");
                writer.WriteLine($"        ___________________________________{cat.CategoryName.Replace(" ", "_").ToUpper()} = {baseID},");
                for (var j = 0; j < cat.Entries.Count; j++)
                {
                    var id = baseID + j + 1;
                    writer.WriteLine($"        {cat.Entries[j].Replace(" ", "")} = {id},");
                }
            }

            writer.WriteLine("    }");
            writer.WriteLine();
            writer.WriteLine("    public static class EntityType");
            writer.WriteLine("    {");

            for (var i = 0; i < _database.Categories.Count; i++)
            {
                var baseID = 100 + i * 100;
                var endID = baseID + 100;
                var catName = _database.Categories[i].CategoryName.Replace(" ", "_");

                writer.WriteLine($"        public static bool Is{catName}(this EntityID entityID)");
                writer.WriteLine("        {");
                writer.WriteLine("            var intValue = (int)entityID;");
                writer.WriteLine($"            return intValue is > {baseID} and < {endID};");
                writer.WriteLine("        }");
                writer.WriteLine();
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        #endregion
    }
}