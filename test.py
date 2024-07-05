

vowels = "aiouy"
alphabet = "azertyuiopqsdfghjklmwxcvbn"

txt = open("Assets/Resources/words.txt").read().split("\n")


for a in alphabet:
    for b in alphabet:
        count = 0
        for word in txt:
            if len(word) >= 4:
                if a + b in word:
                    count += 1

        if count > 50000:
            print(a + b, count)

