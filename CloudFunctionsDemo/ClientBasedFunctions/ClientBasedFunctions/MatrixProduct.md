# Matrix Multiplication

The following describes how matrix multiplication is implemented in this demo. We use the technique described by Halevi 
and Shoup in [Algorithms for HElib (2014)](https://eprint.iacr.org/2014/106). We note that there are multiple ways of 
performing matrix multiplication, with different trade-offs and different complexities. The technique we use here is
suitable for this application, but may not be generalizable to other problems.

The basic principle that we use is that for a matrix-vector product such as
```
[ a b c ] [ A ]
[ d e f ] [ B ]
[ g h i ] [ C ]
```

we form “generalized diagonals” of the matrix and write them as rows of a new matrix

```
cyclicDiagsMatrix = [ a e i ]
                    [ b f g ]
                    [ c d h ]
```

We encrypt the rows of `cyclicDiagsMatrix` into separate ciphertexts and send them to the server as `matrixa`. 
We also encrypt the column vector as a single ciphertext (A, B, C go in the `BatchEncoder` slots); this is sent as `matrixb`.

On the server, we compute element-wise products of the row ciphertexts of `matrixa` with rotations of `matrixb`:

```
[ a e i ] * [ A B C ] = [ aA eB iC ]
[ b f g ] * [ B C A ] = [ bB fC gA ]
[ c d h ] * [ C A B ] = [ cC dA hB ]
```

Finally we sum these together:

```
[ aA eB iC ] + [ bB fC gA ] + [ cC dA hB ] = [ aA+bB+cC eB+fC+dA iC+gA+hB ]
```

The result is now in one single ciphertext in the `BatchEncoder` slots. We send this back as the result; indeed, it 
contains the result of the matrix-vector product.

The number of slots in a batched plaintext/ciphertext equals `PolyModulusDegee` and the slots are organized into 
a 2-by-`PolyModulusDegree`/2 matrix. Using `Evaluator.RotateRows` we can cyclically rotate both rows at the same time, 
and with `Evaluator.RotateColumns` we can swap the two rows. For now, consider using only the first row. 
Since `PolyModulusDegree` is set to 4096 in the demo, we have a row length of 2048. Note that rotations require 
`GaloisKeys` on the server's side; to save space, we generate keys only for the rotations that are actually used.

We only use power-of-two size matrices to get the cyclic rotations to line up easily, since we cannot control the 
slot count or batching row length arbitrarily. For example, in the above case we actually expand the matrix to be 4-by-4:
```
 [ a b c 0 ] [ A ]
 [ d e f 0 ] [ B ]
 [ g h i 0 ] [ C ]
 [ 0 0 0 0 ] [ 0 ]
```

Now we encode the row `[ a b c 0 ]` into a batching row of size 2048 so that each element is separated by 2048/4=512 
slots. The batching array data before encoding and encrypting will therefore look like:
```
            ( a, 0, 0, ..., 0, b, 0, 0, ..., 0, c, 0, 0, ..., 0, 0, 0, 0, 0, ..., 0 )
Position:     0  1            512              1024             1536     2048 onward all zeros (second row of batched plaintext)
```

We encode the vector similarly:
```
            ( A, 0, 0, ..., 0, B, 0, 0, ..., 0, C, 0, 0, ..., 0, 0, 0, 0, 0, ..., 0 )
Position:     0  1            512              1024             1536     2048 onward all zeros (second row of batched plaintext)
```

Now the rotations we need to perform are by 512 slots. Of course, if the matrix size is different, then this will be 
different. On the server's side, it is important to rotate `matrixb` in the right direction: this is the positive direction 
in terms of Microsoft SEAL `Evaluator.RotateRows` API.

It is very easy now to incorporate more columns to `matrixb`:
```
[ a b c 0 ] [ A D ]
[ d e f 0 ] [ B E ]
[ g h i 0 ] [ C F ]
[ 0 0 0 0 ] [ 0 0 ]
```

We use the 512-size gaps to compute the result for all `matrixb` columns at once. To do this, we encode `matrixa` as follows: 
```
            ( a, a, 0, ..., 0, b, b, 0, ..., 0, c, c, 0, ..., 0, 0, 0, 0, 0, 0, ..., 0 )
Position:     0  1            512 513         1024 1024        1536 1537    2048 onward all zeros (second row of batched plaintext)
```
Here the number of repetitions of the value depend on how many columns are in `matrixb`.

We encode `matrixb` as follows:
```
            ( A, D, 0, ..., 0, B, E, 0, ..., 0, C, F, 0, ..., 0, 0, 0, 0, 0, 0, ..., 0 )
Position:     0  1           512 513          1024 1025        1536 1537    2048 onward all zeros (second row of batched plaintext)
```
Now the same matrix multiplication technique works and results in a single ciphertext vector, where the result data is 
in row-major order. The rows will be separated by 512 `BatchEncoder` slots.

Finally, we use two other tricks:
* We also use the second `BatchEncoder` row so that we can send two rows of `matrixa` in a single ciphertext. On the `matrixb` side we encode `matrixb` in the second row as already rotated by 512 steps (in this example). 
* Since communication depends on the number of rows of `matrixa`, if `matrixb` instead has fewer columns than `matrixa` has rows, we instead compute AB = (B^T A^T)^T, where the ^T means matrix transpose, to reduce communication to as small as possible.
