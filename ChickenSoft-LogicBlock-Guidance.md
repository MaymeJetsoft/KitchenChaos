Dans l'architecture Chickensoft, la classe Godot `Counter` (le nœud de la scène) ne contient pas de logique de jeu. Elle implémente les comportements en agissant comme une **vue (View)** et un **pont** : elle initialise le `LogicBlock`, s'abonne à ses sorties (*outputs*), capte les entrées physiques du jeu (comme l'interaction du joueur) et traduit tout cela en modifiant la scène Godot (apparitions d'objets, animations, etc.).

Voici concrètement comment s'implémente un tel composant `Counter` dans Godot avec C#.

### 1. Le nœud Godot `Counter.cs`
Ce script est attaché à la scène de votre comptoir. Il gère le lien entre l'univers Godot et le `CounterLogic` présenté précédemment.

using Godot;

public partial class Counter : StaticBody3D, IInteractable {
    [Export] public Node3D ItemSpawnPoint { get; set; } // Point d'ancrage visuel de l'objet sur le comptoir

    private CounterLogic _logic;
    private ILogicBlockBinding<CounterLogic, CounterLogic.CounterState> _binding;
    
    // Référence vers l'instance 3D de l'objet actuellement affiché (visuel)
    private Node3D _currentVisualItem;

    public override void _Ready() {
        // 1. Instanciation du LogicBlock (le "cerveau" pur C#)
        _logic = new CounterLogic();

        // 2. Création du Binding pour écouter les changements et les sorties du bloc
        _binding = _logic.Bind();

        _binding
            // Écoute des Outputs (effets secondaires / actions uniques)
            .Handle<CounterLogic.CounterState.Output.ItemPlaced>(OnItemPlaced)
            .Handle<CounterLogic.CounterState.Output.ItemTaken>(OnItemTaken);

        // 3. Démarrage de la machine à états
        _logic.Start();
    }

    public override void _ExitTree() {
        // Nettoyage indispensable des liaisons pour éviter les fuites mémoire
        _binding.Dispose();
        _logic.Dispose();
    }

    // --- INTERFACE DE JEU (appelée par le joueur lorsqu'il interagit avec le comptoir) ---
    public void Interact(Player player) {
        // On récupère ce que le joueur tient dans les mains (ex: "Tomato" ou string.Empty)
        string playerItem = player.HeldItemId;

        // On envoie l'intention (Input) au LogicBlock
        _logic.Input(new CounterLogic.CounterState.Input.Interact(playerItem));

        // Note : C'est le LogicBlock qui valide si l'action est possible et 
        // qui émettra un Output en réponse. Le nœud Godot ne décide rien.
    }

    // --- RÉACTIONS AUX SORTIES (Outputs) DU LOGICBLOCK ---

    private void OnItemPlaced(CounterLogic.CounterState.Output.ItemPlaced output) {
        // 1. Mettre à jour l'affichage visuel dans Godot
        _currentVisualItem = LoadItemVisual(output.ItemId);
        ItemSpawnPoint.AddChild(_currentVisualItem);

        // 2. Mettre à jour l'état des mains du joueur (il a posé l'objet)
        // (Géré généralement via un système d'évènements global ou une référence directe)
    }

    private void OnItemTaken(CounterLogic.CounterState.Output.ItemTaken output) {
        // 1. Retirer l'objet visuel du comptoir
        _currentVisualItem?.QueueFree();
        _currentVisualItem = null;

        // 2. Donner l'objet au joueur
        // player.PickUp(output.ItemId);
    }

    private Node3D LoadItemVisual(string itemId) {
        // Charge une scène 3D d'ingrédient en fonction de son ID
        var scene = GD.Load<PackedScene>($"res://assets/items/{itemId}.tscn");
        return scene.Instantiate<Node3D>();
    }
}

### 2. Comment passer à un comportement plus complexe (ex: Un CuttingCounter) ?
L'intérêt majeur de cette approche est que **le code de base du `Counter` ne change presque pas** lorsque les règles du comptoir évoluent.

Si vous voulez transformer ce comptoir en un plan de découpe (`CuttingCounter`) :

1. Vous créez un nouveau `CuttingCounterLogic` (un autre `LogicBlock` qui gère des états supplémentaires : `Empty`, `Occupied`, `Cutting` avec une barre de progression, `Cut`).
2. Le nœud Godot `Counter` peut soit devenir générique en acceptant n'importe quel `ICounterLogic`, soit vous dupliquez/adaptez très légèrement le script du nœud pour écouter de nouveaux `Outputs` (comme `ProgressUpdated` pour animer une barre de progression ou `ItemChopped`).

### En résumé :

- **Le LogicBlock (`CounterLogic`)** calcule **ce qui doit se passer** (règles métier, transitions d'états, validation).
- **Le Nœud Godot (`Counter`)** exécute **comment cela se traduit concrètement** dans le moteur de jeu (instanciation de scènes 3D, déplacements, sons, animations).
- La séparation permet de tester unitairement toute la logique de cuisine en C pur sans jamais avoir besoin de lancer l'éditeur Godot ou de simuler des nœuds graphiques.
