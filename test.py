

vowels = "aiouy"
alphabet = "azertyuiopqsdfghjklmwxcvbn"

txt = open("Assets/Resources/words.txt").read().split("\n")

for a in alphabet:
    for b in alphabet:
        counter = 0
        for word in txt:
            if (a + b) in word:
                counter += 1

        if counter > 10000 and counter < 50000:
            print("\"{}\", ".format((a + b).upper()), end="")

