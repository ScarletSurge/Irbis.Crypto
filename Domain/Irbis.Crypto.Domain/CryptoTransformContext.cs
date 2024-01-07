namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
public sealed class CryptoTransformContext
{
    
    #region Nested
    
    #region Cipher mode
    
    /// <summary>
    /// 
    /// </summary>
    private interface ICipherModeBlockTransformation
    {
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="blocksStartIndex"></param>
        /// <param name="blocksToTransformCount"></param>
        /// <param name="initializationVector"></param>
        void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="blocksStartIndex"></param>
        /// <param name="blocksToTransformCount"></param>
        /// <param name="initializationVector"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default);

    }
    
    /// <summary>
    /// 
    /// </summary>
    private abstract class BaseCipherModeBlockTransformation :
        ICipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        protected BaseCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm)
        {
            SymmetricCipherAlgorithm = symmetricCipherAlgorithm ?? throw new ArgumentNullException(nameof(symmetricCipherAlgorithm));
        }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        protected ISymmetricCipherAlgorithm SymmetricCipherAlgorithm
        {
            get;
        }
        
        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation

        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public abstract void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector);

        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public abstract Task TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default);

        #endregion

    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class ElectronicCodebookCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public ElectronicCodebookCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm) :
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public override void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public override Task TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class CipherBlockChainingCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public CipherBlockChainingCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm) :
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public override void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public override Task<(byte[], byte[])> TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class PropagatingCipherBlockChainingCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public PropagatingCipherBlockChainingCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm) :
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public override void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public override Task<(byte[], byte[])> TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class CipherFeedbackCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public CipherFeedbackCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm) :
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public override void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public override Task<(byte[], byte[])> TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class OutputFeedbackCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public OutputFeedbackCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm) :
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public override void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public override Task<(byte[], byte[])> TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class CounterCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public CounterCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm) :
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public override void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public override Task<(byte[], byte[])> TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class RandomDeltaCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public RandomDeltaCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm) :
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public override void Transform(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public override Task<(byte[], byte[])> TransformAsync(
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[] initializationVector,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    #endregion
    
    #region Padding mode
    
    /// <summary>
    /// 
    /// </summary>
    private interface IPaddingModeBlockTransformation
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        void Transform(
            ref byte[] block);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        void TransformBack(
            ref byte[] block);

    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class ZerosPaddingModeBlockTransformation :
        IPaddingModeBlockTransformation
    {
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.IPaddingModeBlockTransformation implementation

        /// <inheritdoc cref="IPaddingModeBlockTransformation.Transform" />
        public void Transform(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IPaddingModeBlockTransformation.TransformBack" />
        public void TransformBack(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class PKCS7PaddingModeBlockTransformation:
        IPaddingModeBlockTransformation
    {
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.IPaddingModeBlockTransformation implementation

        /// <inheritdoc cref="IPaddingModeBlockTransformation.Transform" />
        public void Transform(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IPaddingModeBlockTransformation.TransformBack" />
        public void TransformBack(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class ANSIX923PaddingModeBlockTransformation:
        IPaddingModeBlockTransformation
    {
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.IPaddingModeBlockTransformation implementation

        /// <inheritdoc cref="IPaddingModeBlockTransformation.Transform" />
        public void Transform(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IPaddingModeBlockTransformation.TransformBack" />
        public void TransformBack(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class ISO10126PaddingModeBlockTransformation:
        IPaddingModeBlockTransformation
    {
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.IPaddingModeBlockTransformation implementation

        /// <inheritdoc cref="IPaddingModeBlockTransformation.Transform" />
        public void Transform(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IPaddingModeBlockTransformation.TransformBack" />
        public void TransformBack(
            ref byte[] block)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
    }

    #endregion
    
    #endregion
    
    #region Fields
    
    /// <summary>
    /// 
    /// </summary>
    private readonly int _coresToUseCount;
    
    /// <summary>
    /// 
    /// </summary>
    private readonly byte[]? _initializationVector;
    
    /// <summary>
    /// 
    /// </summary>
    private readonly ICipherModeBlockTransformation _cipherModeBlockTransformation;
    
    /// <summary>
    /// 
    /// </summary>
    private readonly IPaddingModeBlockTransformation _paddingModeBlockTransformation;

    #endregion
    
    #region Constructors
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="coresToUseCount"></param>
    /// <param name="algorithm"></param>
    /// <param name="cipherMode"></param>
    /// <param name="initializationVector"></param>
    /// <param name="paddingMode"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public CryptoTransformContext(
        int coresToUseCount,
        ISymmetricCipherAlgorithm algorithm,
        CipherMode cipherMode = CipherMode.ElectronicCodebook,
        byte[]? initializationVector = null,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        _coresToUseCount = coresToUseCount <= 0
            ? coresToUseCount
            : throw new ArgumentOutOfRangeException(nameof(coresToUseCount), "Cores to use count can't be LT or EQ to 0.");

        if (cipherMode != CipherMode.ElectronicCodebook)
        {
            _initializationVector = initializationVector ?? throw new ArgumentNullException(nameof(initializationVector));
        }
        
        _cipherModeBlockTransformation = cipherMode switch
        {
            CipherMode.ElectronicCodebook => new ElectronicCodebookCipherModeBlockTransformation(algorithm),
            CipherMode.CipherBlockChaining => new CipherBlockChainingCipherModeBlockTransformation(algorithm),
            CipherMode.PropagatingCipherBlockChaining => new PropagatingCipherBlockChainingCipherModeBlockTransformation(algorithm),
            CipherMode.CipherFeedback => new CipherFeedbackCipherModeBlockTransformation(algorithm),
            CipherMode.OutputFeedback => new OutputFeedbackCipherModeBlockTransformation(algorithm),
            CipherMode.CounterMode => new CounterCipherModeBlockTransformation(algorithm),
            CipherMode.RandomDelta => new RandomDeltaCipherModeBlockTransformation(algorithm),
            _ => throw new ArgumentOutOfRangeException(nameof(cipherMode))
        };

        _paddingModeBlockTransformation = paddingMode switch
        {
            PaddingMode.Zeros => new ZerosPaddingModeBlockTransformation(),
            PaddingMode.PKCS7 => new PKCS7PaddingModeBlockTransformation(),
            PaddingMode.ANSIX923 => new ANSIX923PaddingModeBlockTransformation(),
            PaddingMode.ISO10126 => new ISO10126PaddingModeBlockTransformation(),
            _ => throw new ArgumentOutOfRangeException(nameof(paddingMode))
        };
    }

    #endregion
    
    #region Methods
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputStream"></param>
    /// <returns></returns>
    public byte[] PerformCipher(
        byte[] inputStream)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="token"></param>
    public Task PerformCipherAsync(
        string inputFilePath,
        string outputFilePath,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    
    #endregion

}