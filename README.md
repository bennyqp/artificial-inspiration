# artificial-inspiration

![artificial images header](Demo%20Images/artificial_inspiration_img01.jpg)


### "Artificial Inspiration" is an attempt to stimulate and enhance human creativity in a new way using artificial intelligence to achieve new and more creative results. 

How will it be possible to stimulate and increase human creativity related to visual design by using resources from the field of artificial intelligence to overcome the predictable and achieve innovative, creative results? Will it be possible to break collective thought patterns through targeted, non-human stimuli and create a new level of creativity in the interaction between humans and artificial intelligence? How can we make use of the significant advances in the field of AI in recent years to build the creative process of the future in an enriching way? 

The possibility of mathematically representing complex systems such as language or painting styles enables a completely new approach to a variety of topics. Connections in large amounts of data can be detected and rules can be extracted from them. Based on these rules, data can be combined with each other, resulting in outcomes that exceed what is humanly possible. 

On this basis, the creative process was analyzed and compared with the possibilities of AI. The goal is to generate impulses from the linking of various data and to show new perspectives on existing problems, which inspire people to find new solutions: Artificial Inspiration.

To do this, a theoretical process for increasing creativity with the help of AI was developed, which was then implemented using a practical design task as an example: Finding new and creative ways to represent a portrait. This process essentially consists of two steps: 
\
#### 1. Generate as many different variations of one portrait as possible.
#### 2. Assisting in identifying the drafts that are found to be personally inspiring by an individual.
\

In the first step, new types of portrait variations are to be generated. For this purpose, an initial portrait was first generated with StyleGAN using the trained Celeb-HQ data set: 

![stylegan protrait](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img02.jpg)


In the next step, different algorithms and models were combined in a specific way to manipulate the generated portrait and create new forms of representation. At this point, of course, the combination of models can be extended and rearranged as desired. So there is the possibility of more and more variations.

![combination of models](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img03.jpg)

In this way, over 10000 portrait variants were generated in this example. 

![artificial inspiration images](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img04.jpg)
![artificial inspiration images](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img05.jpg)

In the second instance of the designed process, the images, which have a personal inspiring effect on an individual user, must be found from the multitude of results. To make this possible, all generated images are classified according to various criteria:

- General visual similarity (Img2Vec)
- Style of the image (Img2Vec)
- Color scheme (KMeans) 
- Degree of abstractness (Face Recognition) 


Based on this analysis, a three-dimensional vector is now assigned to each image. Subsequently, a virtual reality application was developed that allows navigation through the three-dimensional space of images. In this application, the images can also be filtered according to the criteria already mentioned and clustered with the help of "filter bombs". This makes it possible to explore different perspectives on the subject of portraits, store inspiring approaches, compare them and develop new ideas from them.

The overriding goal is that the creativity of the user in relation to the subject matter is stimulated by this process and thus novel creative results can be developed. 


The process is exemplified by portrait variations, but can be applied to many other tasks: e.g. the design of logos or posters. There are no limits. 


www.artificial-inspiration.com

![artificial inspiration images](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img06.jpg)
![artificial inspiration images](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img07.jpg)
![artificial inspiration images](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img08.jpg)
![artificial inspiration images](https://github.com/bennyqp/artificial-inspiration/blob/main/Demo%20Images/artificial_inspiration_img09.jpg)
