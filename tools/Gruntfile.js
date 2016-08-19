module.exports = function(grunt) {

  grunt.initConfig({
    replace: {
      date: {
        options: {
          patterns: [
            {
              match: /<VersionDate>.*(?=<\/VersionDate>)/,
              replacement: '<VersionDate>'+grunt.template.today('isoDateTime')+"Z"
            }
          ]
        },
        files: [
          {src: ['package.config'], dest: './'}
        ]
      },
      scversion: {
        options: {
          patterns: [
            {
              match: /<Dependency name="Starcounter">.*(?=<\/Dependency>)/,
              replacement: '<Dependency name="Starcounter">~' + grunt.option('value')
          },
            {
              match: /Requires Starcounter version .*/,
              replacement: 'Requires Starcounter version '+ grunt.option('value')
            }
          ]
        },
        files: [
          {src: ['package.config'], dest: './'},
          {src: ['../README.md'], dest: './'}
        ]
      }
    },
    bump: {
      options: {
        files: ['package.json', 'package.config'],
        commit: true,
        commitMessage: '%VERSION%',
        commitFiles: ['package.json', 'package.config', '../README.md'],
        createTag: true,
        tagName: '%VERSION%',
        tagMessage: 'Version %VERSION%',
        push: false,
        // pushTo: 'origin',
        globalReplace: false,
        prereleaseName: "develop",
        regExp: new RegExp(
          '([\'|\"]?version[\'|\"]?[ ]*:[ ]*[\'|\"]?|<Version>)(\\d+\\.\\d+\\.\\d+(-' +
          "pre" + // opts.prereleaseName +
          '\\.\\d+)?(-\\d+)?)[\\d||A-a|.|-]*([\'|\"]?)', 'i'
        )
      }
    },
    shell:{
      scversion: {
        command: 'star --version',
        options:{
          callback: function(err, stdout, stderr, cb){
            var version = stdout.replace('Version=','');
            // grunt.config.set('scversion', version);
            grunt.util.spawn({
              grunt: true,
              args: ['replace:scversion', '--value='+version]
            }, function(err, res, code) {
              cb();
            });
          }
        }
      },
      package: {
          command: 'package.bat'
      }
    }
  });

  grunt.loadNpmTasks('grunt-replace');
  grunt.loadNpmTasks('grunt-bump');
  grunt.loadNpmTasks('grunt-shell');

  grunt.registerTask(
     'package',
     "Generate Starcounter App Store package with bumped version",
     function(versionType, incOrCommitOnly) {
        grunt.task.run("replace:date", "shell:scversion", "bump" + (versionType ? ":"+versionType : ""), "shell:package");

    }
  );

};
