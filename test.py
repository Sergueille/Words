

vowels = "aiouy"
alphabet = "azertyuiopqsdfghjklmwxcvbn"

txt = open("Assets/Resources/words.txt").read().split("\n")

count = 0
for word in txt:
    if len(word) >= 4:
        if word[0] == word[len(word) - 1]:
            count += 1
print(count)

