namespace DesignPatterns.Behavioral.State
{
    // State interface
    public interface IVendingMachineState
    {
        void InsertCoin(VendingMachine machine);
        void SelectProduct(VendingMachine machine);
        void DispenseProduct(VendingMachine machine);
        void RefundCoins(VendingMachine machine);
        string GetStateName();
    }

    // Context
    public class VendingMachine
    {
        private IVendingMachineState _currentState;
        private int _coinCount;
        private int _productStock;

        public VendingMachine(int productStock)
        {
            _productStock = productStock;
            _coinCount = 0;
            _currentState = new NoCoinState();
        }

        public void SetState(IVendingMachineState state)
        {
            _currentState = state;
            Console.WriteLine($"State changed to: {state.GetStateName()}");
        }

        public int CoinCount => _coinCount;
        public int ProductStock => _productStock;
        public string CurrentState => _currentState.GetStateName();

        public void AddCoin()
        {
            _coinCount++;
            Console.WriteLine($"Coin inserted. Total coins: {_coinCount}");
        }

        public void RemoveCoins()
        {
            Console.WriteLine($"Returning {_coinCount} coins");
            _coinCount = 0;
        }

        public void RemoveProduct()
        {
            if (_productStock > 0)
            {
                _productStock--;
                Console.WriteLine($"Product dispensed. Remaining stock: {_productStock}");
            }
        }

        // Public methods that delegate to current state
        public void InsertCoin() => _currentState.InsertCoin(this);
        public void SelectProduct() => _currentState.SelectProduct(this);
        public void DispenseProduct() => _currentState.DispenseProduct(this);
        public void RefundCoins() => _currentState.RefundCoins(this);
    }

    // Concrete States
    public class NoCoinState : IVendingMachineState
    {
        public string GetStateName() => "No Coin";

        public void InsertCoin(VendingMachine machine)
        {
            machine.AddCoin();
            if (machine.CoinCount >= 1)
            {
                machine.SetState(new HasCoinState());
            }
        }

        public void SelectProduct(VendingMachine machine)
        {
            Console.WriteLine("Please insert a coin first");
        }

        public void DispenseProduct(VendingMachine machine)
        {
            Console.WriteLine("Please insert a coin and select a product first");
        }

        public void RefundCoins(VendingMachine machine)
        {
            Console.WriteLine("No coins to refund");
        }
    }

    public class HasCoinState : IVendingMachineState
    {
        public string GetStateName() => "Has Coin";

        public void InsertCoin(VendingMachine machine)
        {
            machine.AddCoin();
            Console.WriteLine("Additional coin accepted");
        }

        public void SelectProduct(VendingMachine machine)
        {
            if (machine.ProductStock > 0)
            {
                Console.WriteLine("Product selected");
                machine.SetState(new ProductSelectedState());
            }
            else
            {
                Console.WriteLine("Product out of stock");
                machine.SetState(new OutOfStockState());
            }
        }

        public void DispenseProduct(VendingMachine machine)
        {
            Console.WriteLine("Please select a product first");
        }

        public void RefundCoins(VendingMachine machine)
        {
            machine.RemoveCoins();
            machine.SetState(new NoCoinState());
        }
    }

    public class ProductSelectedState : IVendingMachineState
    {
        public string GetStateName() => "Product Selected";

        public void InsertCoin(VendingMachine machine)
        {
            Console.WriteLine("Product already selected, processing...");
        }

        public void SelectProduct(VendingMachine machine)
        {
            Console.WriteLine("Product already selected");
        }

        public void DispenseProduct(VendingMachine machine)
        {
            machine.RemoveProduct();
            machine.RemoveCoins();
            machine.SetState(new NoCoinState());
        }

        public void RefundCoins(VendingMachine machine)
        {
            Console.WriteLine("Cannot refund after product selection. Dispensing product...");
            DispenseProduct(machine);
        }
    }

    public class OutOfStockState : IVendingMachineState
    {
        public string GetStateName() => "Out of Stock";

        public void InsertCoin(VendingMachine machine)
        {
            Console.WriteLine("Machine is out of stock. Coin will be refunded");
            machine.AddCoin();
        }

        public void SelectProduct(VendingMachine machine)
        {
            Console.WriteLine("Machine is out of stock");
        }

        public void DispenseProduct(VendingMachine machine)
        {
            Console.WriteLine("Machine is out of stock");
        }

        public void RefundCoins(VendingMachine machine)
        {
            machine.RemoveCoins();
            machine.SetState(new NoCoinState());
        }
    }

    // Real-world example: Document Workflow
    public interface IDocumentState
    {
        void Edit(Document document);
        void Review(Document document);
        void Approve(Document document);
        void Publish(Document document);
        string GetStateName();
    }

    public class Document
    {
        private IDocumentState _currentState;
        private string _content;

        public Document(string content)
        {
            _content = content;
            _currentState = new DraftState();
        }

        public void SetState(IDocumentState state)
        {
            _currentState = state;
            Console.WriteLine($"Document state changed to: {state.GetStateName()}");
        }

        public string Content => _content;
        public string CurrentState => _currentState.GetStateName();

        public void UpdateContent(string newContent)
        {
            _content = newContent;
            Console.WriteLine($"Document content updated: {_content}");
        }

        // Public methods that delegate to current state
        public void Edit() => _currentState.Edit(this);
        public void Review() => _currentState.Review(this);
        public void Approve() => _currentState.Approve(this);
        public void Publish() => _currentState.Publish(this);
    }

    public class DraftState : IDocumentState
    {
        public string GetStateName() => "Draft";

        public void Edit(Document document)
        {
            Console.WriteLine("Editing document in draft state");
            document.UpdateContent($"{document.Content} [Edited]");
        }

        public void Review(Document document)
        {
            Console.WriteLine("Sending document for review");
            document.SetState(new InReviewState());
        }

        public void Approve(Document document)
        {
            Console.WriteLine("Cannot approve document in draft state. Please send for review first");
        }

        public void Publish(Document document)
        {
            Console.WriteLine("Cannot publish document in draft state");
        }
    }

    public class InReviewState : IDocumentState
    {
        public string GetStateName() => "In Review";

        public void Edit(Document document)
        {
            Console.WriteLine("Document is in review. Sending back to draft for editing");
            document.SetState(new DraftState());
            document.UpdateContent($"{document.Content} [Revised]");
        }

        public void Review(Document document)
        {
            Console.WriteLine("Document is already in review");
        }

        public void Approve(Document document)
        {
            Console.WriteLine("Document approved by reviewer");
            document.SetState(new ApprovedState());
        }

        public void Publish(Document document)
        {
            Console.WriteLine("Cannot publish document while in review");
        }
    }

    public class ApprovedState : IDocumentState
    {
        public string GetStateName() => "Approved";

        public void Edit(Document document)
        {
            Console.WriteLine("Document is approved. Sending back to draft for editing");
            document.SetState(new DraftState());
        }

        public void Review(Document document)
        {
            Console.WriteLine("Document is already approved");
        }

        public void Approve(Document document)
        {
            Console.WriteLine("Document is already approved");
        }

        public void Publish(Document document)
        {
            Console.WriteLine("Publishing approved document");
            document.SetState(new PublishedState());
        }
    }

    public class PublishedState : IDocumentState
    {
        public string GetStateName() => "Published";

        public void Edit(Document document)
        {
            Console.WriteLine("Cannot edit published document. Create a new version");
        }

        public void Review(Document document)
        {
            Console.WriteLine("Document is already published");
        }

        public void Approve(Document document)
        {
            Console.WriteLine("Document is already published");
        }

        public void Publish(Document document)
        {
            Console.WriteLine("Document is already published");
        }
    }

    // Audio Player example
    public interface IAudioPlayerState
    {
        void Play(AudioPlayer player);
        void Pause(AudioPlayer player);
        void Stop(AudioPlayer player);
        string GetStateName();
    }

    public class AudioPlayer
    {
        private IAudioPlayerState _currentState;
        private string _currentTrack;

        public AudioPlayer()
        {
            _currentState = new StoppedState();
            _currentTrack = "";
        }

        public void SetState(IAudioPlayerState state)
        {
            _currentState = state;
            Console.WriteLine($"Audio player state: {state.GetStateName()}");
        }

        public void LoadTrack(string track)
        {
            _currentTrack = track;
            Console.WriteLine($"Loaded track: {track}");
        }

        public string CurrentTrack => _currentTrack;
        public string CurrentState => _currentState.GetStateName();

        public void Play() => _currentState.Play(this);
        public void Pause() => _currentState.Pause(this);
        public void Stop() => _currentState.Stop(this);
    }

    public class StoppedState : IAudioPlayerState
    {
        public string GetStateName() => "Stopped";

        public void Play(AudioPlayer player)
        {
            if (!string.IsNullOrEmpty(player.CurrentTrack))
            {
                Console.WriteLine($"Playing: {player.CurrentTrack}");
                player.SetState(new PlayingState());
            }
            else
            {
                Console.WriteLine("No track loaded");
            }
        }

        public void Pause(AudioPlayer player)
        {
            Console.WriteLine("Cannot pause when stopped");
        }

        public void Stop(AudioPlayer player)
        {
            Console.WriteLine("Already stopped");
        }
    }

    public class PlayingState : IAudioPlayerState
    {
        public string GetStateName() => "Playing";

        public void Play(AudioPlayer player)
        {
            Console.WriteLine("Already playing");
        }

        public void Pause(AudioPlayer player)
        {
            Console.WriteLine($"Paused: {player.CurrentTrack}");
            player.SetState(new PausedState());
        }

        public void Stop(AudioPlayer player)
        {
            Console.WriteLine($"Stopped: {player.CurrentTrack}");
            player.SetState(new StoppedState());
        }
    }

    public class PausedState : IAudioPlayerState
    {
        public string GetStateName() => "Paused";

        public void Play(AudioPlayer player)
        {
            Console.WriteLine($"Resuming: {player.CurrentTrack}");
            player.SetState(new PlayingState());
        }

        public void Pause(AudioPlayer player)
        {
            Console.WriteLine("Already paused");
        }

        public void Stop(AudioPlayer player)
        {
            Console.WriteLine($"Stopped: {player.CurrentTrack}");
            player.SetState(new StoppedState());
        }
    }
}
