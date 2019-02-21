# Microsoft SEAL

[Microsoft SEAL](https://www.microsoft.com/en-us/research/project/microsoft-seal) is a low-level cryptographic library providing an API for encryption, computing on encrypted data, and decryption, using a special encryption technology called homomorphic encryption. Microsoft SEAL is written in C++17 and has no external dependencies. It comes with complete .NET Standard wrappers and multiple thoroughly commented examples. Computations on encrypted data results in encrypted outputs that can be decrypted only by the original data owner, allowing developers to build end-to-end encrypted data storage and computation services where the customer never needs to share their key with the service and is guaranteed that the service provider cannot share the customer’s data with third parties.

Microsoft SEAL can be downloaded from [GitHub](https://GitHub.com/Microsoft/SEAL).

# Microsoft SEAL Demo

Microsoft SEAL Demo presents a set of applications that demonstrate the use of Microsoft SEAL for implementing a variety of development scenarios 
where users' personal data is protected. We show developers and researchers how they can compute on untrusted computing premises (e.g., cloud environments).  

### Current list of demos

1. [Cloud Functions Demo using Microsoft SEAL for .NET](CloudFunctionsDemo/README.md)
