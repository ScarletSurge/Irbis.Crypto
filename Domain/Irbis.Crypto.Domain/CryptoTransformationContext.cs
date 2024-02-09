using System.Numerics;

namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
public static class CryptoTransformationContext
{
    
    #region Nested

    private delegate void SymmetricCipherOperation(byte[] input, ref byte[] output);
    
    #region Cipher mode
    
    /// <summary>
    /// 
    /// </summary>
    private interface ICipherModeBlockTransformation
    {
        
        /// <summary>
        /// 
        /// </summary>
        int BlockSize
        {
            get;
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherTransformationMode"></param>
        /// <param name="blocks"></param>
        /// <param name="blocksStartIndex"></param>
        /// <param name="blocksToTransformCount"></param>
        /// <param name="transformationAdditionalData"></param>
        void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherTransformationMode"></param>
        /// <param name="blocks"></param>
        /// <param name="blocksStartIndex"></param>
        /// <param name="blocksToTransformCount"></param>
        /// <param name="transformationAdditionalData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// 
    /// </summary>
    private abstract class BaseCipherModeBlockTransformation:
        ICipherModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        /// <exception cref="ArgumentNullException"></exception>
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
        
        #region Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherTransformationMode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected SymmetricCipherOperation GetTransformation(
            CipherTransformationMode cipherTransformationMode)
        {
            return cipherTransformationMode switch
            {
                CipherTransformationMode.Encryption => SymmetricCipherAlgorithm.Encrypt,
                CipherTransformationMode.Decryption => SymmetricCipherAlgorithm.Decrypt,
                _ => throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode))
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialBlock"></param>
        /// <param name="additionalBlock"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected void ExclusiveOr(
            byte[] initialBlock,
            byte[] additionalBlock)
        {
            _ = initialBlock ?? throw new ArgumentNullException(nameof(initialBlock));
            _ = additionalBlock ?? throw new ArgumentNullException(nameof(additionalBlock));
            if (initialBlock.Length != additionalBlock.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(additionalBlock));
            }

            for (var i = 0; i < initialBlock.Length; i++)
            {
                initialBlock[i] ^= additionalBlock[i];
            }
        }
        
        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.ICipherModeBlockTransformation implementation
        
        /// <inheritdoc cref="ICipherModeBlockTransformation.BlockSize" />
        public int BlockSize =>
            SymmetricCipherAlgorithm.BlockSize;

        /// <inheritdoc cref="ICipherModeBlockTransformation.Transform" />
        public abstract void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData);

        /// <inheritdoc cref="ICipherModeBlockTransformation.TransformAsync" />
        public abstract Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default);

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
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm):
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BaseCipherModeBlockTransformation overrides
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.Transform" />
        public override void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData)
        {
            var transformation = GetTransformation(cipherTransformationMode);
            
            Parallel.For(blocksStartIndex, blocksStartIndex + blocksToTransformCount, blockIndex =>
            {
                transformation(blocks[blockIndex], ref blocks[blockIndex]);
            });
        }
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.TransformAsync" />
        public override Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default)
        {
            var transformation = GetTransformation(cipherTransformationMode);
            
            return Task.WhenAll(blocks.Skip(blocksStartIndex).Take(blocksToTransformCount).Select(block => Task.Run(
                () => transformation(block, ref block), cancellationToken)));
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class CipherBlockChainingCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Fields
        
        /// <summary>
        /// 
        /// </summary>
        private readonly byte[][] _temporaryBlocksForDecryptionOperation;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        /// <param name="coresToUseCount"></param>
        public CipherBlockChainingCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm,
            int coresToUseCount):
            base(symmetricCipherAlgorithm)
        {
            _temporaryBlocksForDecryptionOperation = new byte[coresToUseCount][];
            for (var i = 0; i < coresToUseCount; i++)
            {
                _temporaryBlocksForDecryptionOperation[i] = new byte[symmetricCipherAlgorithm.BlockSize];
            }
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BaseCipherModeBlockTransformation overrides
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.Transform" />
        public override void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData)
        {
            var transformation = GetTransformation(cipherTransformationMode);
            
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                        transformation(blocks[blockIndex], ref blocks[blockIndex]);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    Parallel.For(blocksStartIndex, blocksStartIndex + blocksToTransformCount, blockIndex =>
                    {
                        transformation(blocks[blockIndex], ref _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]);
                    });
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        ExclusiveOr(_temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length], transformationAdditionalData);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                        _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length].CopyTo(blocks[blockIndex], 0);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }
        }
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.TransformAsync" />
        public override async Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default)
        {
            var transformation = GetTransformation(cipherTransformationMode);
            
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                        transformation(blocks[blockIndex], ref blocks[blockIndex]);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    await Task.WhenAll(blocks.Skip(blocksStartIndex).Take(blocksToTransformCount).Select((block, blockIndex) => Task.Run(() => transformation(block, ref _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]), cancellationToken)));
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ExclusiveOr(_temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length], transformationAdditionalData);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                        _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length].CopyTo(blocks[blockIndex], 0);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class PropagatingCipherBlockChainingCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Fields
        
        /// <summary>
        /// 
        /// </summary>
        private readonly byte[] _temporaryTransformationAdditionalData;
        
        /// <summary>
        /// 
        /// </summary>
        private readonly byte[][] _temporaryBlocksForDecryptionOperation;

        #endregion
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        /// <param name="coresToUseCount"></param>
        public PropagatingCipherBlockChainingCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm,
            int coresToUseCount):
            base(symmetricCipherAlgorithm)
        {
            _temporaryTransformationAdditionalData = new byte[symmetricCipherAlgorithm.BlockSize];
            _temporaryBlocksForDecryptionOperation = new byte[coresToUseCount][];
            for (var i = 0; i < coresToUseCount; i++)
            {
                _temporaryBlocksForDecryptionOperation[i] = new byte[symmetricCipherAlgorithm.BlockSize];
            }
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BaseCipherModeBlockTransformation overrides
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.Transform" />
        public override void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData)
        {
            var transformation = GetTransformation(cipherTransformationMode);
            
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        blocks[blockIndex].CopyTo(_temporaryTransformationAdditionalData, 0);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                        _temporaryTransformationAdditionalData.CopyTo(transformationAdditionalData, 0);
                        transformation(blocks[blockIndex], ref blocks[blockIndex]);
                        ExclusiveOr(transformationAdditionalData, blocks[blockIndex]);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    Parallel.For(blocksStartIndex, blocksStartIndex + blocksToTransformCount, blockIndex =>
                    {
                        blocks[blockIndex].CopyTo(_temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length], 0);
                        transformation(blocks[blockIndex], ref _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]);
                    });
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        ExclusiveOr(_temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length], transformationAdditionalData);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                        ExclusiveOr(transformationAdditionalData, _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]);
                        _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length].CopyTo(blocks[blockIndex], 0);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }
        }
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.TransformAsync" />
        public override async Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default)
        {
            var transformation = GetTransformation(cipherTransformationMode);
            
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        blocks[blockIndex].CopyTo(_temporaryTransformationAdditionalData, 0);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                        _temporaryTransformationAdditionalData.CopyTo(transformationAdditionalData, 0);
                        transformation(blocks[blockIndex], ref blocks[blockIndex]);
                        ExclusiveOr(transformationAdditionalData, blocks[blockIndex]);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    await Task.WhenAll(blocks.Skip(blocksStartIndex).Take(blocksToTransformCount).Select((block, blockIndex) => Task.Run(() => transformation(block, ref _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]), cancellationToken)));
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ExclusiveOr(_temporaryBlocksForDecryptionOperation[blockIndex], transformationAdditionalData);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                        _temporaryBlocksForDecryptionOperation[blockIndex].CopyTo(blocks[blockIndex], 0);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class CipherFeedbackCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Fields
        
        /// <summary>
        /// 
        /// </summary>
        private readonly byte[][] _temporaryBlocksForDecryptionOperation;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        /// <param name="coresToUseCount"></param>
        public CipherFeedbackCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm,
            int coresToUseCount):
            base(symmetricCipherAlgorithm)
        {
            _temporaryBlocksForDecryptionOperation = new byte[coresToUseCount][];
            for (var i = 0; i < coresToUseCount; i++)
            {
                _temporaryBlocksForDecryptionOperation[i] = new byte[symmetricCipherAlgorithm.BlockSize];
            }
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BaseCipherModeBlockTransformation overrides
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.Transform" />
        public override void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData)
        {
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        SymmetricCipherAlgorithm.Encrypt(transformationAdditionalData, ref transformationAdditionalData);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    Parallel.For(blocksStartIndex, blocksStartIndex + blocksToTransformCount, blockIndex =>
                    {
                        var blockToEncrypt = blockIndex == blocksStartIndex
                            ? transformationAdditionalData
                            : blocks[blockIndex - 1];
                        SymmetricCipherAlgorithm.Encrypt(blockToEncrypt, ref _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]);
                        ExclusiveOr(blocks[blockIndex], _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }
        }
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.TransformAsync" />
        public override async Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default)
        {
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SymmetricCipherAlgorithm.Encrypt(transformationAdditionalData, ref transformationAdditionalData);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                        blocks[blockIndex].CopyTo(transformationAdditionalData, 0);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    await Task.WhenAll(blocks.Skip(blocksStartIndex).Take(blocksToTransformCount).Select((block, blockIndex) => Task.Run(() =>
                    {
                        var blockToEncrypt = blockIndex == blocksStartIndex
                            ? transformationAdditionalData
                            : blocks[blockIndex - 1];
                        SymmetricCipherAlgorithm.Encrypt(blockToEncrypt, ref _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]);
                        ExclusiveOr(blockToEncrypt, _temporaryBlocksForDecryptionOperation[blockIndex % _temporaryBlocksForDecryptionOperation.Length]);
                    }, cancellationToken)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }
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
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm):
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BaseCipherModeBlockTransformation overrides
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.Transform" />
        public override void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData)
        {
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        SymmetricCipherAlgorithm.Encrypt(transformationAdditionalData, ref transformationAdditionalData);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        SymmetricCipherAlgorithm.Encrypt(transformationAdditionalData, ref transformationAdditionalData);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }
        }
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.TransformAsync" />
        public override Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default)
        {
            switch (cipherTransformationMode)
            {
                case CipherTransformationMode.Encryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SymmetricCipherAlgorithm.Encrypt(transformationAdditionalData, ref transformationAdditionalData);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                    }
                    break;
                case CipherTransformationMode.Decryption:
                    for (var blockIndex = blocksStartIndex; blockIndex < blocksStartIndex + blocksToTransformCount; blockIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SymmetricCipherAlgorithm.Encrypt(transformationAdditionalData, ref transformationAdditionalData);
                        ExclusiveOr(blocks[blockIndex], transformationAdditionalData);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cipherTransformationMode));
            }

            return Task.CompletedTask;
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class CounterCipherModeBlockTransformation:
        BaseCipherModeBlockTransformation
    {
        
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly byte[][] _temporaryBlocksForCipherOperation;

        #endregion
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        /// <param name="coresToUseCount"></param>
        public CounterCipherModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm,
            int coresToUseCount):
            base(symmetricCipherAlgorithm)
        {
            _temporaryBlocksForCipherOperation = new byte[coresToUseCount + 1][];
            for (var i = 0; i <= coresToUseCount; i++)
            {
                _temporaryBlocksForCipherOperation[i] = new byte[symmetricCipherAlgorithm.BlockSize];
            }
        }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bigIntegerValueArray"></param>
        /// <param name="spanByteArray"></param>
        /// <returns></returns>
        private BigInteger GetBigInteger(
            byte[] bigIntegerValueArray,
            byte[] spanByteArray)
        {
            var span = new Span<byte>(spanByteArray, 0, spanByteArray.Length);
            span.Clear();
            bigIntegerValueArray.CopyTo(span);
            return new BigInteger(span);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="spanByteArray"></param>
        /// <param name="isUnsigned"></param>
        /// <param name="isBigEndian"></param>
        private void SetBigInteger(
            BigInteger value,
            byte[] spanByteArray,
            bool isUnsigned = false,
            bool isBigEndian = false)
        {
            var span = new Span<byte>(spanByteArray, 0, spanByteArray.Length);
            span.Clear();
            value.TryWriteBytes(span, out _, isUnsigned, isBigEndian);
        }
        
        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BaseCipherModeBlockTransformation overrides
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.Transform" />
        public override void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData)
        {
            var counterValue = GetBigInteger(transformationAdditionalData, _temporaryBlocksForCipherOperation[^1]);
            var mask = (BigInteger.One << (transformationAdditionalData.Length << 3)) - BigInteger.One;
            var counterNewValue = (counterValue + blocksToTransformCount) & mask;
            
            Parallel.For(blocksStartIndex, blocksStartIndex + blocksToTransformCount, blockIndex =>
            {
                var blockToCipherIndex = blockIndex % (_temporaryBlocksForCipherOperation.Length - 1);
                var counterValueForCurrentThread = (counterValue + blockIndex - blocksStartIndex) & mask;
                var counterValueForCurrentThreadSpan = new Span<byte>(_temporaryBlocksForCipherOperation[blockToCipherIndex], 0, _temporaryBlocksForCipherOperation[blockToCipherIndex].Length);
                counterValueForCurrentThreadSpan.Clear();
                counterValueForCurrentThread.TryWriteBytes(counterValueForCurrentThreadSpan, out _, true);
                SymmetricCipherAlgorithm.Encrypt(_temporaryBlocksForCipherOperation[blockToCipherIndex], ref _temporaryBlocksForCipherOperation[blockToCipherIndex]);
                ExclusiveOr(blocks[blockIndex], _temporaryBlocksForCipherOperation[blockToCipherIndex]);
            });
            
            SetBigInteger(counterNewValue, transformationAdditionalData, true);
        }
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.TransformAsync" />
        public override async Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default)
        {
            var counterValue = GetBigInteger(transformationAdditionalData, _temporaryBlocksForCipherOperation[^1]);
            var mask = (BigInteger.One << (transformationAdditionalData.Length << 3)) - BigInteger.One;
            var counterNewValue = (counterValue + blocksToTransformCount) & mask;
            
            await Task.WhenAll(blocks.Skip(blocksStartIndex).Take(blocksToTransformCount).Select((block, blockIndex) => Task.Run(() =>
            {
                var blockToCipherIndex = blockIndex % (_temporaryBlocksForCipherOperation.Length - 1);
                var counterValueForCurrentThread = (counterValue + blockIndex - blocksStartIndex) & mask;
                var counterValueForCurrentThreadSpan = new Span<byte>(_temporaryBlocksForCipherOperation[blockToCipherIndex], 0, _temporaryBlocksForCipherOperation[blockToCipherIndex].Length);
                counterValueForCurrentThreadSpan.Clear();
                counterValueForCurrentThread.TryWriteBytes(counterValueForCurrentThreadSpan, out _, true);
                SymmetricCipherAlgorithm.Encrypt(_temporaryBlocksForCipherOperation[blockToCipherIndex], ref _temporaryBlocksForCipherOperation[blockToCipherIndex]);
                ExclusiveOr(blocks[blockIndex], _temporaryBlocksForCipherOperation[blockToCipherIndex]);
            }, cancellationToken)));
            
            SetBigInteger(counterNewValue, transformationAdditionalData, true);
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
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm):
            base(symmetricCipherAlgorithm)
        {
            
        }

        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BaseCipherModeBlockTransformation overrides
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.Transform" />
        public override void Transform(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc cref="BaseCipherModeBlockTransformation.TransformAsync" />
        public override Task TransformAsync(
            CipherTransformationMode cipherTransformationMode,
            byte[][] blocks,
            int blocksStartIndex,
            int blocksToTransformCount,
            byte[]? transformationAdditionalData,
            CancellationToken cancellationToken = default)
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
        /// <param name="blockInitialSize"></param>
        void Transform(
            ref byte[] block,
            int? blockInitialSize = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="forceResize"></param>
        void TransformBack(
            ref byte[] block,
            bool forceResize = false);

    }
    
    /// <summary>
    /// 
    /// </summary>
    private abstract class BasePaddingModeBlockTransformation:
        IPaddingModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BasePaddingModeBlockTransformation(
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
        
        #region Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="validateBlockSizeLTTarget"></param>
        /// <param name="validateBlockSizeGTTarget"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private BasePaddingModeBlockTransformation ThrowIfInvalidBlock(
            byte[] block,
            bool validateBlockSizeLTTarget = true,
            bool validateBlockSizeGTTarget = true)
        {
            _ = block ?? throw new ArgumentNullException(nameof(block));
            
            if (validateBlockSizeLTTarget && block.Length < SymmetricCipherAlgorithm.BlockSize)
            {
                throw new ArgumentException("Block is too short.", nameof(block));
            }
            
            if (validateBlockSizeGTTarget && block.Length > SymmetricCipherAlgorithm.BlockSize)
            {
                throw new ArgumentException("Block is too long.", nameof(block));
            }

            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="blockInitialSize"></param>
        protected abstract void TransformInner(
            ref byte[] block,
            int? blockInitialSize = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="forceResize"></param>
        protected abstract void TransformBackInner(
            ref byte[] block,
            bool forceResize = false);

        #endregion

        #region Irbis.Crypto.Domain.CryptoTransformContext.IPaddingModeBlockTransformation implementation
        
        /// <inheritdoc cref="IPaddingModeBlockTransformation.Transform" />
        public void Transform(
            ref byte[] block,
            int? blockInitialSize = null)
        {
            ThrowIfInvalidBlock(block, false).TransformInner(ref block, blockInitialSize);
        }
        
        /// <inheritdoc cref="IPaddingModeBlockTransformation.TransformBack" />
        public void TransformBack(
            ref byte[] block,
            bool forceResize = false)
        {
            ThrowIfInvalidBlock(block).TransformBackInner(ref block, forceResize);
        }

        #endregion

    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class ZerosPaddingModeBlockTransformation:
        BasePaddingModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public ZerosPaddingModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm):
            base(symmetricCipherAlgorithm)
        {
            
        }
        
        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BasePaddingModeBlockTransformation overrides

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformInner" />
        protected override void TransformInner(
            ref byte[] block,
            int? blockInitialSize = null)
        {
            if ((blockInitialSize ?? block.Length) == SymmetricCipherAlgorithm.BlockSize)
            {
                return;
            }
            
            Array.Resize(ref block, SymmetricCipherAlgorithm.BlockSize);
        }

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformBackInner" />
        protected override void TransformBackInner(
            ref byte[] block,
            bool forceResize = false)
        {
            var bytesToRemoveCount = 0;
            for (; bytesToRemoveCount < SymmetricCipherAlgorithm.BlockSize; bytesToRemoveCount++)
            {
                if (block[SymmetricCipherAlgorithm.BlockSize - 1 - bytesToRemoveCount] != 0)
                {
                    break;
                }
            }

            if (bytesToRemoveCount == 0)
            {
                return;
            }

            if (!forceResize)
            {
                return;
            }
            
            Array.Resize(ref block, block.Length - bytesToRemoveCount);
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class PKCS7PaddingModeBlockTransformation:
        BasePaddingModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public PKCS7PaddingModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm):
            base(symmetricCipherAlgorithm)
        {
            
        }
        
        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BasePaddingModeBlockTransformation overrides

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformInner" />
        protected override void TransformInner(
            ref byte[] block,
            int? blockInitialSize = null)
        {
            if ((blockInitialSize ?? block.Length) == SymmetricCipherAlgorithm.BlockSize)
            {
                return;
            }

            var difference = (byte)(SymmetricCipherAlgorithm.BlockSize - block.Length);
            Array.Resize(ref block, SymmetricCipherAlgorithm.BlockSize);
            Array.Fill(block, difference, block.Length - difference, difference);
        }

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformBackInner" />
        protected override void TransformBackInner(
            ref byte[] block,
            bool forceResize = false)
        {
            var lastByteValue = block[^1];
            
            if (lastByteValue >= SymmetricCipherAlgorithm.BlockSize || lastByteValue == 0)
            {
                return;
            }

            for (var i = 1; i < lastByteValue; i++)
            {
                if (block[block.Length - 1 - i] != lastByteValue)
                {
                    return;
                }
            }

            if (!forceResize)
            {
                return;
            }
            
            Array.Resize(ref block, block.Length - lastByteValue);
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class ANSIX923PaddingModeBlockTransformation:
        BasePaddingModeBlockTransformation
    {
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public ANSIX923PaddingModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm):
            base(symmetricCipherAlgorithm)
        {
            
        }
        
        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BasePaddingModeBlockTransformation overrides

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformInner" />
        protected override void TransformInner(
            ref byte[] block,
            int? blockInitialSize = null)
        {
            if ((blockInitialSize ?? block.Length) == SymmetricCipherAlgorithm.BlockSize)
            {
                return;
            }

            var difference = (byte)(SymmetricCipherAlgorithm.BlockSize - block.Length);
            Array.Resize(ref block, SymmetricCipherAlgorithm.BlockSize);
            block[^1] = difference;
        }

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformBackInner" />
        protected override void TransformBackInner(
            ref byte[] block,
            bool forceResize = false)
        {
            var lastByteValue = block[^1];
            
            if (lastByteValue >= SymmetricCipherAlgorithm.BlockSize || lastByteValue == 0)
            {
                return;
            }

            for (var i = 1; i < lastByteValue; i++)
            {
                if (block[block.Length - 1 - i] != 0)
                {
                    return;
                }
            }
            
            if (!forceResize)
            {
                return;
            }
            
            Array.Resize(ref block, block.Length - lastByteValue);
        }
        
        #endregion
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class ISO10126PaddingModeBlockTransformation:
        BasePaddingModeBlockTransformation
    {
        
        #region Fields
        
        /// <summary>
        /// 
        /// </summary>
        private readonly Random _randomSource;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symmetricCipherAlgorithm"></param>
        public ISO10126PaddingModeBlockTransformation(
            ISymmetricCipherAlgorithm symmetricCipherAlgorithm):
            base(symmetricCipherAlgorithm)
        {
            _randomSource = new Random();
        }
        
        #endregion
        
        #region Irbis.Crypto.Domain.CryptoTransformContext.BasePaddingModeBlockTransformation overrides

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformInner" />
        protected override void TransformInner(
            ref byte[] block,
            int? blockInitialSize = null)
        {
            if ((blockInitialSize ?? block.Length) == SymmetricCipherAlgorithm.BlockSize)
            {
                return;
            }
            
            var difference = (byte)(SymmetricCipherAlgorithm.BlockSize - block.Length);
            Array.Resize(ref block, SymmetricCipherAlgorithm.BlockSize);
            _randomSource.NextBytes(new Span<byte>(block, block.Length - difference, difference - 1));
            block[^1] = difference;
        }

        /// <inheritdoc cref="BasePaddingModeBlockTransformation.TransformBackInner" />
        protected override void TransformBackInner(
            ref byte[] block,
            bool forceResize = false)
        {
            var lastByteValue = block[^1];
            
            if (lastByteValue >= SymmetricCipherAlgorithm.BlockSize || lastByteValue == 0)
            {
                return;
            }

            if (!forceResize)
            {
                return;
            }
            
            Array.Resize(ref block, block.Length - lastByteValue);
        }
        
        #endregion
        
    }

    #endregion
    
    #endregion
    
    #region Methods
    
    
    public static byte[] PerformCipher(
        ISymmetricCipherAlgorithm algorithm,
        int coresToUseCount,
        CipherMode cipherMode,
        PaddingMode paddingMode,
        byte[] inputStream,
        CipherTransformationMode cipherTransformationMode,
        byte[]? initializationVector)
    {
        _ = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        
        if (coresToUseCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coresToUseCount), "Cores to use count must be GT 0.");
        }
        
        if (cipherMode != CipherMode.ElectronicCodebook && initializationVector == null)
        {
            throw new ArgumentNullException(nameof(initializationVector));
        }
        if (algorithm.BlockSize != (initializationVector?.Length ?? algorithm.BlockSize))
        {
            throw new ArgumentOutOfRangeException(nameof(initializationVector), "Initialization vector length must be EQ to algorithm's block size.");
        }
        
        ICipherModeBlockTransformation cipherModeBlockTransformation = cipherMode switch
        {
            CipherMode.ElectronicCodebook => new ElectronicCodebookCipherModeBlockTransformation(algorithm),
            CipherMode.CipherBlockChaining => new CipherBlockChainingCipherModeBlockTransformation(algorithm, coresToUseCount),
            CipherMode.PropagatingCipherBlockChaining => new PropagatingCipherBlockChainingCipherModeBlockTransformation(algorithm, coresToUseCount),
            CipherMode.CipherFeedback => new CipherFeedbackCipherModeBlockTransformation(algorithm, coresToUseCount),
            CipherMode.OutputFeedback => new OutputFeedbackCipherModeBlockTransformation(algorithm),
            CipherMode.CounterMode => new CounterCipherModeBlockTransformation(algorithm, coresToUseCount),
            CipherMode.RandomDelta => new RandomDeltaCipherModeBlockTransformation(algorithm),
            _ => throw new ArgumentOutOfRangeException(nameof(cipherMode))
        };

        IPaddingModeBlockTransformation paddingModeBlockTransformation = paddingMode switch
        {
            PaddingMode.Zeros => new ZerosPaddingModeBlockTransformation(algorithm),
            PaddingMode.PKCS7 => new PKCS7PaddingModeBlockTransformation(algorithm),
            PaddingMode.ANSIX923 => new ANSIX923PaddingModeBlockTransformation(algorithm),
            PaddingMode.ISO10126 => new ISO10126PaddingModeBlockTransformation(algorithm),
            _ => throw new ArgumentOutOfRangeException(nameof(paddingMode))
        };
        
        var blocks = new byte[coresToUseCount][];
        for (var i = 0; i < coresToUseCount; i++)
        {
            blocks[i] = new byte[cipherModeBlockTransformation.BlockSize];
        }
        var byteIndex = 0;

        while (byteIndex < inputStream.Length)
        {
            var blocksToTransformCount = 0;
            
            for (; blocksToTransformCount < coresToUseCount; blocksToTransformCount++)
            {
                var sourceIndex = byteIndex + blocksToTransformCount * cipherModeBlockTransformation.BlockSize;
                if (sourceIndex >= inputStream.Length)
                {
                    break;
                }
                
                Array.Copy(inputStream, sourceIndex, blocks[blocksToTransformCount], 0, cipherModeBlockTransformation.BlockSize);
            }
            
            cipherModeBlockTransformation.Transform(cipherTransformationMode, blocks, 0, blocksToTransformCount, initializationVector);

            for (var i = 0; i < coresToUseCount; i++)
            {
                Array.Copy(blocks[i], 0, inputStream, byteIndex + i * cipherModeBlockTransformation.BlockSize, cipherModeBlockTransformation.BlockSize);
            }
            
            byteIndex += coresToUseCount * cipherModeBlockTransformation.BlockSize;
        }

        return inputStream;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="cancellationToken"></param>
    public static Task PerformCipherAsync(
        string inputFilePath,
        string outputFilePath,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    #endregion

}