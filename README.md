# PasswordHashingShowcase

This repo demonstrates the usage of a proper password hashing and storage.


## What is hashing?

Hashing is basically a one-way cryptographic function generaly used to produce a digest to help
identify if two blocks of data are the same. It is irreversible which means that knowing the
result of a hashing function should not allow you to recreate the contents of said block of
data.

Because of that, "_collision_" becomes a concern. _collision_ refers to hashing two different 
values with the same hashing function resulting in the same output. Since hashing is expected 
to have as low collision as possible, the lowest the collision chance the better the algorithm
trustworthiness.

You may send a text though the internet and need to guarantee that it has not been tempered 
with, you can use a hashing function to provide the digest to your clients so they can check for
themselves.

Say you have sent a text file with content: `notapasswordatall`. To provide a proof to your clients
that what they are receiving is what you have sent them you can provide a digest allongside it.
Using SHA-256, for example, it does not metter how many times you run the function on the file
the output will always be `be44eca306d45dd46c5114c4e76438c0095704904d79edda4f327a1297e07585`

## What is _password hashing_?

The premise is the same, it is a one-way (irreversible) cryptographic function. However, it has 
enfasis on safely storing passwords and has focus on making infeasable to break a password hash
if it is ever discovered.

Even though the premise is the same, a hashing function should not be confused with a spetialized
password hashing function. A hashing funcion used to create file digest **should never** be used
to store passwords and **are not recomended** for such practices.

It is not hard to find information about the best and latest password hashing functions in the
market. For example [bcrypt](https://bcrypt.online/) has been in the market since 1999, and after
the crypto race boom, it has gained a lot of attention, that is not necessarily good since now
there is hardware specifically made to crack hashes created with this algorithm. After that, came
[scrypt](https://www.tarsnap.com/scrypt.html) and brought a lot to the table with an algorithm
that costs approximately 4000 times more to crack then bcrypt and is currently a good pick, but
still not the best.

However, the best to date might be [Argon2](https://github.com/p-h-c/phc-winner-argon2), the 
winner of PHC 2015 and also a great password hashing algorithm.

The last two are currently the better known and tested out there. And there is also a good library
that exposes these algorithms and some more funcionality, [libsodium](https://doc.libsodium.org/).

Some really usefull tool can be found [here](https://esse.tools/).

## Know how to make it right

Even thought these algorithms are great they are not 100% garanteed to keep you safe from a 
data breach (may that never happen to you). Dictionary attacks are still possible and rainbow tables
still exist. Thats when we have to spice things a bit. (Got it? Spice things up with pepper... 
Like the thing we add to the passwo... Argh... Nevermind).

### Safeguard 1: Salt

Not like kitchen salt but the premise is almost the same: we add it onto something to make it 
better.
Salt is a random chuck of data we attach to the password being hashed. 
So, for example, say you have a password `weakpwd`, when creating an account we could add a salt
to it; so now it would be `weakpwd57982`.

Using bcrypt with a cost factor of 10:

- Your raw password hash would be: `$2y$10$1yA93au7BP.RPjSNVR8tYe1Aj2I16d6W/UeCC4dWysBS0p8xS1bxy`
- Your salted password hash would be: `$2y$10$ZtlBfKWe9ecVR5geLMyipOxE/T/.i417tic2KHG1wD6ZQCDX1t0ES`

Note how after the third dollar sign the values are different (anything before that is used by the
algorithm to know how to get to the same value it gave you).

A cool thing about salt is that you can store it in plaintext in the database along with their
respective passwords. Since salt is intended to help mitigate rainbow-table attacks, the fact that
each password has its own salt could render unfeasble the creation of rainbow-tables to crack a 
password even if the salt is known.

And just like kitchen salt **it should not be reused**. Create a new one for each password and save
it, using the same salt for all passwords goes against its purpoise.

#### How to create a good salt

A GUID would be a good start, it has a great length and incredibly small collision chance
but there are definetly better ways of doing it.
You can always hash a GUID with SHA-3 (Please, do not use MD5!), but any data greater then 16 
bytes is a great start too.

In C# you can use `RandomNumberGenerator.GetBytes()` in `System.Security.Cryptography` to get a 
_cryptographically strong random sequence of values_ (as per the documentation). A salt do not need
to be intelligible, it is better not to be and thats why the `RandomNumberGenerator` method is 
so good for this.

Now that you have a salt attach it to the data you are about to hash, or send it to the hashing
function if it supports it (I.e: Argon2 and Scrypt). The final product can be saved as binary data
in the database (that is, raw bytes), but to make it portable and even to save some space store it
as a Base64 string.

### Safeguard 2: Pepper

##### If salt already made it hard enough for attackers to crack a password, a _pepper_ would centantly make it even worse.

Pepper refers to another chunck of data appended to the password being saved; so you could say
it is the same as a salt but there is one key diference: a pepper is not stored in the same place
as the salt neither is it one per password.

Again with the food analogy, one does not simply add many hands of pepper to a dish, instead it is
carefully added little by little. If too much your dish might not be as edible as you would expect.
When securing our passwords, we should follow the same premise.

Say you have a microservice (hurray... buzzwords!) resposible for authenticating your clients.
Said microservice would have a pepper, a single immutable block of data saved in the application 
that gets appended to every new password about to be hashed, just make an effort to not hardcode 
it as it is never a good idea to hardcode security related information.

Back to your good and old password `weakpwd`. We have already added a salt to it so now it is
`weakpwd57982`. We can go further and add a pepper, and now it is `weakpwd57982%&2jd*@*`.

Thats is the whole point of this. Consider your password like a bet and these two actions as if we
where doubling, or even tripling, that bet. You have a weak 6 characters password? No problem, 
here, take a unique 32 bytes attachment and my 32 bytes personal signature!

## Conclusion
##### One size does not fit all

Althought it all might seem cool, the obvious must be stated: easily guessed passwords are still
a threat. There is only so much we can do to prevent data being breached or stolen, but there must
always be a two-sided effort to prevent these; a client with with knowledge of how to keep their 
information safe and a server willing to meet the clients expectations regarding security.
